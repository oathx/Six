using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.PigeonCoopUtil;
using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
    public abstract class TrailRenderer_Base : MonoBehaviour
    {
        public PCTrailRendererData TrailData;
        public bool Emit = false;
        public int MaxNumberOfPoints = 50;

        protected bool _emit;
        private CircularBuffer<PCTrailPoint> _activeTrail;
        private List<CircularBuffer<PCTrailPoint>> _fadingTrails;

        private List<Mesh> _toCleanUp = new List<Mesh>();

        protected Transform _t;
        
        protected virtual void Awake()
        {
            _activeTrail = new CircularBuffer<PCTrailPoint>(MaxNumberOfPoints);
            _fadingTrails = new List<CircularBuffer<PCTrailPoint>>();
            _t = transform;
            _emit = Emit;
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void LateUpdate()
        {
            if (_fadingTrails == null)
            {
                _fadingTrails = new List<CircularBuffer<PCTrailPoint>>();
            }

            foreach (Mesh mesh in _toCleanUp)
            {
                Destroy(mesh);
            }

            _toCleanUp.Clear();

            CheckEmitChange();

            if(_activeTrail != null)
            {
                UpdatePoints(Time.deltaTime, _activeTrail);

                Mesh trailMesh = GenerateMesh(_activeTrail);
                if(trailMesh != null)
                {
                    DrawMesh(trailMesh);
                    _toCleanUp.Add(trailMesh);  
                } 
            }
            
             
            for (int i = _fadingTrails.Count-1; i >= 0; i--)
            {
                CircularBuffer<PCTrailPoint> trail = _fadingTrails[i];
                if (trail == null || trail.Any(a => a.TimeActive() < TrailData.Lifetime) == false)
                {
                    _fadingTrails.RemoveAt(i);
                    continue;
                }
                UpdatePoints(Time.deltaTime, trail);
                Mesh trailMesh = GenerateMesh(trail);
                if (trailMesh != null)
                {
                    DrawMesh(trailMesh);
                    _toCleanUp.Add(trailMesh); 
                }
            }
        }

        protected virtual void OnStopEmit()
        {
            
        }

        protected virtual void OnStartEmit()
        {
        }

        protected virtual void Reset()
        {
            if(TrailData == null)
                TrailData = new PCTrailRendererData();

            TrailData.ColorOverLife = new Gradient();
            TrailData.Lifetime = 1;
            TrailData.SizeOverLife = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
            MaxNumberOfPoints = 50;
        }

        protected virtual void InitialiseNewPoint(PCTrailPoint newPoint)
        {

        }

        protected virtual void UpdatePoint(PCTrailPoint point, float deltaTime)
        {

        }

        protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
        {
            if (_activeTrail == null)
                return;

            newPoint.Position = pos;
            newPoint.PointNumber = _activeTrail.Count == 0 ? 0 : _activeTrail[_activeTrail.Count - 1].PointNumber + 1;
            InitialiseNewPoint(newPoint);

            newPoint.SetDistanceFromStart(_activeTrail.Count == 0
                                              ? 0
                                              : _activeTrail[_activeTrail.Count - 1].GetDistanceFromStart() + Vector3.Distance(_activeTrail[_activeTrail.Count - 1].Position, pos));

            if(TrailData.UseForwardOverride)
            {
                newPoint.Forward = TrailData.ForwardOverideRelative
                                       ? _t.TransformDirection(TrailData.ForwardOverride.normalized)
                                       : TrailData.ForwardOverride.normalized;
            }

            _activeTrail.Add(newPoint);
        }

        private Mesh GenerateMesh(CircularBuffer<PCTrailPoint> trail)
        {
            Vector3 camForward = Camera.main != null ? Camera.main.transform.forward : Vector3.forward;

            if(TrailData.UseForwardOverride)
            {
                camForward = TrailData.ForwardOverride.normalized;
            }

            Mesh generatedMesh = new Mesh();
            int activePointCount = NumberOfActivePoints(trail);

            if (activePointCount < 2)
                return null;

            Vector3[] verticies = new Vector3[2 * activePointCount];
            Vector3[] normals = new Vector3[2 * activePointCount];
            Vector2[] uvs = new Vector2[2 * activePointCount];
            Color[] colors = new Color[2 * activePointCount];
            int[] indicies = new int[2 * (activePointCount) * 3];


            int vertIndex = 0;
            for (int i = 0; i < trail.Count; i++)
            {
                PCTrailPoint p = trail[i];
                float timeAlong = p.TimeActive()/TrailData.Lifetime;

                if(p.TimeActive() > TrailData.Lifetime)
                {
                    continue;
                }

                if (TrailData.UseForwardOverride && TrailData.ForwardOverideRelative)
                    camForward = p.Forward;

                Vector3 cross = Vector3.zero;

                if (i < trail.Count - 1)
                {
                    cross =
                        Vector3.Cross((trail[i + 1].Position - p.Position).normalized, camForward).
                            normalized;
                }
                else
                {
                    cross =
                        Vector3.Cross((p.Position - trail[i - 1].Position).normalized, camForward).
                            normalized;
                }

                Color c = TrailData.StretchToFit ? TrailData.ColorOverLife.Evaluate(1 - ((float)vertIndex / (float)activePointCount / 2f)) : TrailData.ColorOverLife.Evaluate(timeAlong);
                float s = TrailData.StretchToFit ? TrailData.SizeOverLife.Evaluate(1 - ((float)vertIndex / (float)activePointCount / 2f)) : TrailData.SizeOverLife.Evaluate(timeAlong);
                verticies[vertIndex] = p.Position + cross * s;

                if(TrailData.MaterialTileLength <= 0)
                {
                    uvs[vertIndex] = new Vector2((float)vertIndex / (float)activePointCount / 2f, 0);
                }
                else
                {
                    uvs[vertIndex] = new Vector2(p.GetDistanceFromStart() / TrailData.MaterialTileLength, 0);
                }

                normals[vertIndex] = camForward;
                colors[vertIndex] = c;
                vertIndex++;
                verticies[vertIndex] = p.Position - cross * s;

                if (TrailData.MaterialTileLength <= 0)
                {
                    uvs[vertIndex] = new Vector2((float)vertIndex / (float)activePointCount / 2f, 1);
                }
                else
                {
                    uvs[vertIndex] = new Vector2(p.GetDistanceFromStart() / TrailData.MaterialTileLength, 1);
                }

                normals[vertIndex] = camForward;
                colors[vertIndex] = c;

                vertIndex++;
            }

            int indIndex = 0;
            for (int pointIndex = 0; pointIndex < 2 * (activePointCount - 1); pointIndex++)
            {
                if(pointIndex%2==0)
                {
                    indicies[indIndex] = pointIndex;
                    indIndex++;
                    indicies[indIndex] = pointIndex + 1;
                    indIndex++;
                    indicies[indIndex] = pointIndex + 2;
                }
                else
                {
                    indicies[indIndex] = pointIndex + 2;
                    indIndex++;
                    indicies[indIndex] = pointIndex + 1;
                    indIndex++;
                    indicies[indIndex] = pointIndex;
                }

                indIndex++;
            }

            generatedMesh.vertices = verticies;
            generatedMesh.SetIndices(indicies,MeshTopology.Triangles,0);
            generatedMesh.uv = uvs;
            generatedMesh.normals = normals;
            generatedMesh.colors = colors;
            return generatedMesh;
        }

        private void DrawMesh(Mesh trailMesh)
        {
            Graphics.DrawMesh(trailMesh, Matrix4x4.identity, TrailData.TrailMaterial, gameObject.layer);
        }

        private void UpdatePoints(float deltaTime, CircularBuffer<PCTrailPoint> line)
        {
            foreach (PCTrailPoint pcTrailPoint in line)
            {
                pcTrailPoint.Update(deltaTime);
                UpdatePoint(pcTrailPoint, deltaTime);
            }
        }

        private void CheckEmitChange()
        {
            if (_emit != Emit)
            {
                _emit = Emit;
                if (_emit)
                {
                    OnStartEmit();
                    _activeTrail = new CircularBuffer<PCTrailPoint>(MaxNumberOfPoints);
                }
                else
                {
                    OnStopEmit();
                    _fadingTrails.Add(_activeTrail);
                    _activeTrail = null;
                }
            }
        }

        private int NumberOfActivePoints(CircularBuffer<PCTrailPoint> line)
        {
            int count = 0;
            foreach (PCTrailPoint point in line)
            {
                if (point.TimeActive() < TrailData.Lifetime) count++;
            }
            return count;
        }

        /// <summary>
        /// Insert a trail into this trail renderer. 
        /// </summary>
        /// <param name="from">The start position of the trail.</param>
        /// <param name="to">The end position of the trail.</param>
        /// <param name="distanceBetweenPoints">Distance between each point on the trail</param>
        public void CreateTrail(Vector3 from, Vector3 to, float distanceBetweenPoints)
        {
            float distanceBetween = Vector3.Distance(from, to);

            Vector3 dirVector = to - from;
            dirVector = dirVector.normalized;

            float currentLength = 0;

            CircularBuffer<PCTrailPoint> newLine = new CircularBuffer<PCTrailPoint>(MaxNumberOfPoints);
            int pointNumber = 0;
            while (currentLength < distanceBetween) 
            {
                PCTrailPoint newPoint = new PCTrailPoint();
                newPoint.PointNumber = pointNumber;
                newPoint.Position = from + dirVector*currentLength;
                newLine.Add(newPoint);
                InitialiseNewPoint(newPoint);

                pointNumber++;

                if (distanceBetweenPoints <= 0)
                    break;
                else
                    currentLength += distanceBetweenPoints;
            }

            PCTrailPoint lastPoint = new PCTrailPoint();
            lastPoint.PointNumber = pointNumber;
            lastPoint.Position = to;
            newLine.Add(lastPoint);
            InitialiseNewPoint(lastPoint);

            _fadingTrails.Add(newLine);
        }
        
        /// <summary>
        /// Clears all active trails from the system.
        /// </summary>
        /// <param name="emitState">Desired emit state after clearing</param>
        public void ClearSystem(bool emitState)
        {
            _activeTrail = null;
            if (_fadingTrails != null)
                _fadingTrails.Clear();

            if (_toCleanUp != null)
            {
                foreach (Mesh mesh in _toCleanUp)
                {
                    Destroy(mesh);
                }

                _toCleanUp.Clear();
            }

            Emit = emitState;
            _emit = !emitState;

            CheckEmitChange();
        }

        /// <summary>
        /// Get the number of active seperate trail segments.
        /// </summary>
        public int NumSegments()
        {
            int num = 0;
            if (_activeTrail != null && NumberOfActivePoints(_activeTrail) != 0)
                num++;

            num += _fadingTrails.Count;
            return num;
        }
    }

    public class PCTrailPoint
    {
        public Vector3 Forward;
        public Vector3 Position;
        public int PointNumber;

        private float _timeActive = 0;
        private float _distance;

        public virtual void Update(float deltaTime)
        {
            _timeActive += deltaTime;
        }

        public float TimeActive()
        {
            return _timeActive;
        }

        public void SetDistanceFromStart(float distance)
        {
            _distance = distance;
        }

        public float GetDistanceFromStart()
        {
            return _distance;
        }
    }

    [System.Serializable]
    public class PCTrailRendererData
    {
        public Material TrailMaterial;
        public float Lifetime = 1;
        public AnimationCurve SizeOverLife = new AnimationCurve();
        public Gradient ColorOverLife;
        public float MaterialTileLength = 0;
        public bool StretchToFit;
        public bool UseForwardOverride;
        public Vector3 ForwardOverride;
        public bool ForwardOverideRelative;
    }
}
