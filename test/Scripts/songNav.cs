using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using System;
using System.IO;
using System.Collections.Generic;
using SVector3 = SharpNav.Geometry.Vector3;

namespace Simulate
{
    public class SongNav
    {
        
        public SongNav(string path)
        {
            /*
            * 1. 读取三角网格文件
            * 2. 生成导航网格
            * 
            * */

            //读取地图
            LoadLevel(path);
            GenerateNavMesh();
            //GeneratePathfinding();
        }

        public ObjModel level;
        private int levelNumInds;
        private bool levelHasNorm;
        //读取地图
        private void LoadLevel(string path)
        {
            level = new ObjModel(path);
            var levelTris = level.GetTriangles();
            var levelNorms = level.GetNormals();
            levelNumInds = levelTris.Length * 3;
            levelHasNorm = levelNorms != null && levelNorms.Length > 0;

            //得到包围盒，应该没用着
            //var bounds = TriangleEnumerable.FromTriangle(levelTris, 0, levelTris.Length).GetBoundingBox();
        }

        private NavMeshGenerationSettings settings;
        //Generate poly mesh
        private Heightfield heightfield;
        private CompactHeightfield compactHeightfield;
        private ContourSet contourSet;
        public PolyMesh polyMesh;
        public PolyMeshDetail polyMeshDetail;

        //private void GenerateNavMesh()
        //{
        //    Console.WriteLine("Generating NavMesh");

        //    long prevMs = 0;
        //    try
        //    {
        //        var levelTris = level.GetTriangles();
        //        var triEnumerable = TriangleEnumerable.FromTriangle(levelTris, 0, levelTris.Length);
        //        BBox3 bounds = triEnumerable.GetBoundingBox();

        //        settings = NavMeshGenerationSettings.Default;
        //        heightfield = new Heightfield(bounds, settings);

        //        heightfield.RasterizeTriangles(levelTris, Area.Default);
        //        heightfield.FilterLedgeSpans(settings.VoxelAgentHeight, settings.VoxelMaxClimb);
        //        heightfield.FilterLowHangingWalkableObstacles(settings.VoxelMaxClimb);
        //        heightfield.FilterWalkableLowHeightSpans(settings.VoxelAgentHeight);
        //        compactHeightfield = new CompactHeightfield(heightfield, settings);
        //        compactHeightfield.Erode(settings.VoxelAgentRadius);
        //        compactHeightfield.BuildDistanceField();
        //        compactHeightfield.BuildRegions(0, settings.MinRegionSize, settings.MergedRegionSize);

        //        contourSet = compactHeightfield.BuildContourSet(settings);

        //        polyMesh = new PolyMesh(contourSet, settings);

        //        polyMeshDetail = new PolyMeshDetail(polyMesh, compactHeightfield, settings);


        //        buildData = new NavMeshBuilder(polyMesh, polyMeshDetail, new SharpNav.Pathfinding.OffMeshConnection[0], settings);
        //        tiledNavMesh = new TiledNavMesh(buildData);
        //        navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);
        //    }
        //    catch (Exception e)
        //    {
        //        //if (!interceptExceptions)
        //        //    throw;
        //        //else
        //        //    Console.WriteLine("Navmesh generation failed with exception:" + Environment.NewLine + e.ToString());
        //    }
        //    finally
        //    {
        //        //sw.Stop();
        //    }
        //}


        private void GenerateNavMesh()
        {
            Console.WriteLine("Generating NavMesh");

            long prevMs = 0;
            //try
            //{
                var levelTris = level.GetTriangles();
                var triEnumerable = TriangleEnumerable.FromTriangle(levelTris, 0, levelTris.Length);
                BBox3 bounds = triEnumerable.GetBoundingBox();

                settings = NavMeshGenerationSettings.Default;
                heightfield = new Heightfield(bounds, settings);

                heightfield.RasterizeTriangles(levelTris, Area.Default);
                heightfield.FilterLedgeSpans(settings.VoxelAgentHeight, settings.VoxelMaxClimb);
                heightfield.FilterLowHangingWalkableObstacles(settings.VoxelMaxClimb);
                heightfield.FilterWalkableLowHeightSpans(settings.VoxelAgentHeight);
                compactHeightfield = new CompactHeightfield(heightfield, settings);
                compactHeightfield.Erode(settings.VoxelAgentRadius);
                compactHeightfield.BuildDistanceField();
                compactHeightfield.BuildRegions(0, settings.MinRegionSize, settings.MergedRegionSize);

                contourSet = compactHeightfield.BuildContourSet(settings);

                polyMesh = new PolyMesh(contourSet, settings);

                polyMeshDetail = new PolyMeshDetail(polyMesh, compactHeightfield, settings);


                buildData = new NavMeshBuilder(polyMesh, polyMeshDetail, new SharpNav.Pathfinding.OffMeshConnection[0], settings);
                tiledNavMesh = new TiledNavMesh(buildData);
                navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);
                OutMesh();
            //}
            //catch (Exception e)
            //{
            //    //if (!interceptExceptions)
            //    //    throw;
            //    //else
            //    //    Console.WriteLine("Navmesh generation failed with exception:" + Environment.NewLine + e.ToString());
            //}
            //finally
            //{
            //    //sw.Stop();
            //}
        }

        //private NavMeshCreateParams parameters;
        private NavMeshBuilder buildData;
        public TiledNavMesh tiledNavMesh;
        public NavMeshQuery navMeshQuery;
        //Smooth path for a single unit
        private NavPoint startPt;
        private NavPoint endPt;
        private SharpNav.Pathfinding.Path path;
        private List<SVector3> smoothPath;

        public void GeneratePathfinding()
        {
            
            /*  生成路径寻找
             *  1. 根据polymesh
             * 
             */
            Random rand = new Random();
            NavQueryFilter filter = new NavQueryFilter();
            //1. 
            buildData = new NavMeshBuilder(polyMesh, polyMeshDetail, new SharpNav.Pathfinding.OffMeshConnection[0], settings);

            tiledNavMesh = new TiledNavMesh(buildData);

            OutMesh();


            navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);


            //Find random start and end points on the poly mesh
            /*int startRef;
			navMeshQuery.FindRandomPoint(out startRef, out startPos);*/

            SVector3 c = new SVector3(21.4f, 0, -122f);
            SVector3 e = new SVector3(5, 5, 5);
            navMeshQuery.FindNearestPoly(ref c, ref e, out startPt);

            //navMeshQuery.FindRandomPointAroundCircle(ref startPt, 1000, out endPt);
            //endPt = new NavPoint(new NavPolyId(20), new SVector3(-10, -2, 10));

            c = new SVector3(150.7f, 0, -59f);
            e = new SVector3(5, 5, 5);
            navMeshQuery.FindNearestPoly(ref c, ref e, out endPt);

            //calculate the overall path, which contains an array of polygon references
            int MAX_POLYS = 256;
            path = new SharpNav.Pathfinding.Path();
            navMeshQuery.FindPath(ref startPt, ref endPt, filter, path);

            //find a smooth path over the mesh surface
            int npolys = path.Count;
            SVector3 iterPos = new SVector3();
            SVector3 targetPos = new SVector3();
            navMeshQuery.ClosestPointOnPoly(startPt.Polygon, startPt.Position, ref iterPos);
            navMeshQuery.ClosestPointOnPoly(path[npolys - 1], endPt.Position, ref targetPos);

            //smoothPath = new List<SVector3>(2048);
            //smoothPath.Add(iterPos);

            //float STEP_SIZE = 5f;
            //float SLOP = 0.01f;
            //while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
            //{
            //    //find location to steer towards
            //    SVector3 steerPos = new SVector3();
            //    StraightPathFlags steerPosFlag = 0;
            //    NavPolyId steerPosRef = NavPolyId.Null;

            //    if (!GetSteerTarget(navMeshQuery, iterPos, targetPos, SLOP, path, ref steerPos, ref steerPosFlag, ref steerPosRef))
            //        break;

            //    bool endOfPath = (steerPosFlag & StraightPathFlags.End) != 0 ? true : false;
            //    bool offMeshConnection = (steerPosFlag & StraightPathFlags.OffMeshConnection) != 0 ? true : false;

            //    //find movement delta
            //    SVector3 delta = steerPos - iterPos;
            //    float len = (float)Math.Sqrt(SVector3.Dot(delta, delta));

            //    //if steer target is at end of path or off-mesh link

            //    //don't move past location
            //    if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
            //        len = 1;
            //    else
            //        len = STEP_SIZE / len;

            //    SVector3 moveTgt = new SVector3();
            //    VMad(ref moveTgt, iterPos, delta, len);

            //    //move
            //    SVector3 result = new SVector3();
            //    List<NavPolyId> visited = new List<NavPolyId>(16);
            //    NavPoint startPoint = new NavPoint(path[0], iterPos);
            //    navMeshQuery.MoveAlongSurface(ref startPoint, ref moveTgt, out result, visited);
            //    path.FixupCorridor(visited);
            //    npolys = path.Count;
            //    float h = 0;
            //    navMeshQuery.GetPolyHeight(path[0], result, ref h);
            //    result.Y = h;
            //    iterPos = result;

            //    //handle end of path when close enough
            //    if (endOfPath && InRange(iterPos, steerPos, SLOP, 1.0f))
            //    {
            //        //reached end of path
            //        iterPos = targetPos;
            //        if (smoothPath.Count < smoothPath.Capacity)
            //        {
            //            smoothPath.Add(iterPos);
            //        }
            //        break;
            //    }

            //    //store results
            //    if (smoothPath.Count < smoothPath.Capacity)
            //    {
            //        smoothPath.Add(iterPos);
            //    }
            //}

            StraightPath corners = new StraightPath();
            FindCorners(startPt.Position, endPt.Position, corners, navMeshQuery);
        }

        public void OutMesh()
        {
            string tmpPath = "mmesh.obj";
            System.IO.StreamWriter tmpStreamWriter = new System.IO.StreamWriter(tmpPath);



            //顶点
            //tmpStreamWriter.WriteLine(tmpNavMeshTriangulation.vertices.Length.ToString());
            for (int i = 0; i < polyMeshDetail.Verts.Length; i++)
            {
                tmpStreamWriter.WriteLine("v " + polyMeshDetail.Verts[i].X + " " + polyMeshDetail.Verts[i].Y + " " + polyMeshDetail.Verts[i].Z);
            }

            //tmpStreamWriter.WriteLine(tmpNavMeshTriangulation.indices.Length);
            for (int i = 0; i < polyMeshDetail.MeshCount; i++)
            {

                PolyMeshDetail.MeshData m = polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {

                    var t = polyMeshDetail.Tris[triIndex + j];

                    var v = polyMeshDetail.Verts[vertIndex + t.VertexHash0];

                    v = polyMeshDetail.Verts[vertIndex + t.VertexHash1];

                    v = polyMeshDetail.Verts[vertIndex + t.VertexHash2];
                    tmpStreamWriter.WriteLine("f " + (vertIndex + t.VertexHash0 + 1) + " " + (vertIndex + t.VertexHash1 + 1) + " " + (vertIndex + t.VertexHash2 + 1));
                }

            }
            tmpStreamWriter.Flush();
            tmpStreamWriter.Close();
        }

        //为了使接口适应而附加的
        public List<Vector2> Pathfinding(Vector2 start,Vector2 target)
        {
            StraightPath sp = Pathfinding(new SVector3(start.x_, 0,start.y_),new SVector3(target.x_,0,target.y_));
            List<Vector2> _path = new List<Vector2>();
            for(int i=0;i<sp.Count;i++)
            {
                _path.Add(new Vector2(sp[i].Point.Position.X, sp[i].Point.Position.Z));
                //_path.Add(new Vector2(sp[i].X, sp[i].Y));
            }
            return _path;
        }
        //为了使接口适应而附加的
        public List<Vector2> SmothPathfinding(Vector2 start, Vector2 target)
        {
            Random rand = new Random();
            SVector3 startVector = new SVector3(start.x_,0,start.y_);
            SVector3 endVector = new SVector3(target.x_, 0, target.y_);
            NavQueryFilter filter = new NavQueryFilter();
            //buildData = new NavMeshBuilder(polyMesh, polyMeshDetail, new SharpNav.Pathfinding.OffMeshConnection[0], settings);
            //tiledNavMesh = new TiledNavMesh(buildData);
            //navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);
            SVector3 extents = new SVector3(5, 5, 5);
            navMeshQuery.FindNearestPoly(ref startVector, ref extents, out startPt);
            navMeshQuery.FindNearestPoly(ref endVector, ref extents, out endPt);

            path = new SharpNav.Pathfinding.Path();
            navMeshQuery.FindPath(ref startPt, ref endPt, filter, path);

            //find a smooth path over the mesh surface
            int npolys = path.Count;
            SVector3 iterPos = new SVector3();
            SVector3 targetPos = new SVector3();
            navMeshQuery.ClosestPointOnPoly(startPt.Polygon, startPt.Position, ref iterPos);
            navMeshQuery.ClosestPointOnPoly(path[npolys - 1], endPt.Position, ref targetPos);

            smoothPath = new List<SVector3>(2048);
            smoothPath.Add(iterPos);

            float STEP_SIZE = 2f;
            float SLOP = 0.1f;
            while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
            {
                //find location to steer towards
                SVector3 steerPos = new SVector3();
                StraightPathFlags steerPosFlag = 0;
                NavPolyId steerPosRef = NavPolyId.Null;

                if (!GetSteerTarget(navMeshQuery, iterPos, targetPos, SLOP, path, ref steerPos, ref steerPosFlag, ref steerPosRef))
                    break;

                bool endOfPath = (steerPosFlag & StraightPathFlags.End) != 0 ? true : false;
                bool offMeshConnection = (steerPosFlag & StraightPathFlags.OffMeshConnection) != 0 ? true : false;

                //find movement delta
                SVector3 delta = steerPos - iterPos;
                float len = (float)Math.Sqrt(SVector3.Dot(delta, delta));

                //if steer target is at end of path or off-mesh link

                //don't move past location
                if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
                    len = 1;
                else
                    len = STEP_SIZE / len;

                SVector3 moveTgt = new SVector3();
                VMad(ref moveTgt, iterPos, delta, len);

                //move
                SVector3 result = new SVector3();
                List<NavPolyId> visited = new List<NavPolyId>(16);
                NavPoint startPoint = new NavPoint(path[0], iterPos);
                navMeshQuery.MoveAlongSurface(ref startPoint, ref moveTgt, out result, visited);
                path.FixupCorridor(visited);
                npolys = path.Count;
                float h = 0;
                navMeshQuery.GetPolyHeight(path[0], result, ref h);
                result.Y = h;
                iterPos = result;

                //handle end of path when close enough
                if (endOfPath && InRange(iterPos, steerPos, SLOP, 1.0f))
                {
                    //reached end of path
                    iterPos = targetPos;
                    if (smoothPath.Count < smoothPath.Capacity)
                    {
                        smoothPath.Add(iterPos);
                    }
                    break;
                }

                //store results
                if (smoothPath.Count < smoothPath.Capacity)
                {
                    smoothPath.Add(iterPos);
                }
            }
            List<Vector2> _path = new List<Vector2>();

            for (int i = 0; i < smoothPath.Count; i++)
            {
                _path.Add(new Vector2(smoothPath[i].X, smoothPath[i].Z));
            }
            return _path;
        }

        //只找Corner
        public StraightPath Pathfinding(SVector3 startVector, SVector3 endVector)
        {
            Random rand = new Random();
            NavQueryFilter filter = new NavQueryFilter();
            //buildData = new NavMeshBuilder(polyMesh, polyMeshDetail, new SharpNav.Pathfinding.OffMeshConnection[0], settings);
            //tiledNavMesh = new TiledNavMesh(buildData);
            //navMeshQuery = new NavMeshQuery(tiledNavMesh, 2048);
            SVector3 extents = new SVector3(5, 5, 5);
            navMeshQuery.FindNearestPoly(ref startVector, ref extents, out startPt);
            navMeshQuery.FindNearestPoly(ref endVector, ref extents, out endPt);

            path = new SharpNav.Pathfinding.Path();
            navMeshQuery.FindPath(ref startPt, ref endPt, filter, path);

            ////find a smooth path over the mesh surface
            //int npolys = path.Count;
            //SVector3 iterPos = new SVector3();
            //SVector3 targetPos = new SVector3();
            //navMeshQuery.ClosestPointOnPoly(startPt.Polygon, startPt.Position, ref iterPos);
            //navMeshQuery.ClosestPointOnPoly(path[npolys - 1], endPt.Position, ref targetPos);

            //smoothPath = new List<SVector3>(2048);
            //smoothPath.Add(iterPos);

            //float STEP_SIZE = 1f;
            //float SLOP = 0.01f;
            //while (npolys > 0 && smoothPath.Count < smoothPath.Capacity)
            //{
            //    //find location to steer towards
            //    SVector3 steerPos = new SVector3();
            //    StraightPathFlags steerPosFlag = 0;
            //    NavPolyId steerPosRef = NavPolyId.Null;

            //    if (!GetSteerTarget(navMeshQuery, iterPos, targetPos, SLOP, path, ref steerPos, ref steerPosFlag, ref steerPosRef))
            //        break;

            //    bool endOfPath = (steerPosFlag & StraightPathFlags.End) != 0 ? true : false;
            //    bool offMeshConnection = (steerPosFlag & StraightPathFlags.OffMeshConnection) != 0 ? true : false;

            //    //find movement delta
            //    SVector3 delta = steerPos - iterPos;
            //    float len = (float)Math.Sqrt(SVector3.Dot(delta, delta));

            //    //if steer target is at end of path or off-mesh link

            //    //don't move past location
            //    if ((endOfPath || offMeshConnection) && len < STEP_SIZE)
            //        len = 1;
            //    else
            //        len = STEP_SIZE / len;

            //    SVector3 moveTgt = new SVector3();
            //    VMad(ref moveTgt, iterPos, delta, len);

            //    //move
            //    SVector3 result = new SVector3();
            //    List<NavPolyId> visited = new List<NavPolyId>(16);
            //    NavPoint startPoint = new NavPoint(path[0], iterPos);
            //    navMeshQuery.MoveAlongSurface(ref startPoint, ref moveTgt, out result, visited);
            //    path.FixupCorridor(visited);
            //    npolys = path.Count;
            //    float h = 0;
            //    navMeshQuery.GetPolyHeight(path[0], result, ref h);
            //    result.Y = h;
            //    iterPos = result;

            //    //handle end of path when close enough
            //    if (endOfPath && InRange(iterPos, steerPos, SLOP, 1.0f))
            //    {
            //        //reached end of path
            //        iterPos = targetPos;
            //        if (smoothPath.Count < smoothPath.Capacity)
            //        {
            //            smoothPath.Add(iterPos);
            //        }
            //        break;
            //    }

            //    //store results
            //    if (smoothPath.Count < smoothPath.Capacity)
            //    {
            //        smoothPath.Add(iterPos);
            //    }
            //}
            StraightPath corners = new StraightPath();
            FindCorners(startVector, endVector, corners, navMeshQuery);
            return corners;
            //return smoothPath;
        }


        public void FindCorners(SVector3 pos, SVector3 target, StraightPath corners, NavMeshQuery navquery)
        {
            const float MinTargetDist = 0.01f;

            navquery.FindStraightPath(pos, target, path, corners, 0);

            //prune points in the beginning of the path which are too close
            while (corners.Count > 0)
            {
                if (((corners[0].Flags & StraightPathFlags.OffMeshConnection) != 0) ||
                    Vector3Extensions.Distance2D(corners[0].Point.Position, pos) > MinTargetDist)
                    break;

                corners.RemoveAt(0);
            }

            //prune points after an off-mesh connection
            for (int i = 0; i < corners.Count; i++)
            {
                if ((corners[i].Flags & StraightPathFlags.OffMeshConnection) != 0)
                {
                    corners.RemoveRange(i + 1, corners.Count - i);
                    break;
                }
            }
        }



        List<Vector2> _points = new List<Vector2>();
        List<Line>_lines = new List<Line>();
        public List<Line> GetObstacle()
        {
            //tmpStreamWriter.WriteLine(tmpNavMeshTriangulation.vertices.Length.ToString());
            for (int i = 0; i < polyMeshDetail.Verts.Length; i++)
            {
                //tmpStreamWriter.WriteLine("v " + polyMeshDetail.Verts[i].X + " " + polyMeshDetail.Verts[i].Y + " " + polyMeshDetail.Verts[i].Z);
                _points.Add(new Vector2( polyMeshDetail.Verts[i].X, polyMeshDetail.Verts[i].Z));
            }

            //tmpStreamWriter.WriteLine(tmpNavMeshTriangulation.indices.Length);
            for (int i = 0; i < polyMeshDetail.MeshCount; i++)
            {

                PolyMeshDetail.MeshData m = polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {

                    var t = polyMeshDetail.Tris[triIndex + j];

                    var v = polyMeshDetail.Verts[vertIndex + t.VertexHash0];

                    v = polyMeshDetail.Verts[vertIndex + t.VertexHash1];

                    v = polyMeshDetail.Verts[vertIndex + t.VertexHash2];

                    //tmpStreamWriter.WriteLine("f " + (vertIndex + t.VertexHash0 + 1) + " " + (vertIndex + t.VertexHash1 + 1) + " " + (vertIndex + t.VertexHash2 + 1));
                    LinesFromTriangle(_points[vertIndex + t.VertexHash0], _points[vertIndex + t.VertexHash1], _points[vertIndex + t.VertexHash2]);
                }

            }
         

            for (int i = 0; i < _lines.Count; i++)
            {
                //if(_lines[i].valid == true)
                {
                    for (int ii = i + 1; ii < _lines.Count; ii++)
                    {
                        if (_lines[i] == _lines[ii])// && _lines[ii].valid == true)
                        {
                            _lines[i].valid = false;
                            _lines[ii].valid = false;
                        }
                    }
                }
            }
            return _lines;
        }

        public void LinesFromTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            _lines.Add(new Line(p1, p2));
            _lines.Add(new Line(p2, p3));
            _lines.Add(new Line(p3, p1));
        }

        /// <summary>
		/// Scaled vector addition
		/// </summary>
		/// <param name="dest">Result</param>
		/// <param name="v1">Vector 1</param>
		/// <param name="v2">Vector 2</param>
		/// <param name="s">Scalar</param>
		private void VMad(ref SVector3 dest, SVector3 v1, SVector3 v2, float s)
        {
            dest.X = v1.X + v2.X * s;
            dest.Y = v1.Y + v2.Y * s;
            dest.Z = v1.Z + v2.Z * s;
        }

        private bool GetSteerTarget(NavMeshQuery navMeshQuery, SVector3 startPos, SVector3 endPos, float minTargetDist, SharpNav.Pathfinding.Path path,
            ref SVector3 steerPos, ref StraightPathFlags steerPosFlag, ref NavPolyId steerPosRef)
        {
            StraightPath steerPath = new StraightPath();
            //navMeshQuery.FindStraightPath(startPos, endPos, path, steerPath, 0);
            navMeshQuery.FindCenterPath(startPos, endPos, path, steerPath, 0);
            int nsteerPath = steerPath.Count;
            if (nsteerPath == 0)
                return false;

            //find vertex far enough to steer to
            int ns = 0;
            while (ns < nsteerPath)
            {
                if ((steerPath[ns].Flags & StraightPathFlags.OffMeshConnection) != 0 ||
                    !InRange(steerPath[ns].Point.Position, startPos, minTargetDist, 1000.0f))
                    break;

                ns++;
            }

            //failed to find good point to steer to
            if (ns >= nsteerPath)
                return false;

            steerPos = steerPath[ns].Point.Position;
            steerPos.Y = startPos.Y;
            steerPosFlag = steerPath[ns].Flags;
            if (steerPosFlag == StraightPathFlags.None && ns == (nsteerPath - 1))
                steerPosFlag = StraightPathFlags.End; // otherwise seeks path infinitely!!!
            steerPosRef = steerPath[ns].Point.Polygon;

            return true;
        }

        private bool InRange(SVector3 v1, SVector3 v2, float r, float h)
        {
            float dx = v2.X - v1.X;
            float dy = v2.Y - v1.Y;
            float dz = v2.Z - v1.Z;
            return (dx * dx + dz * dz) < (r * r) && Math.Abs(dy) < h;
        }
        



        //public void UpdateMoveRequest()
        //{
        //    const int PATH_MAX_AGENTS = 8;
        //    Agent[] queue = new Agent[PATH_MAX_AGENTS];
        //    int numQueue = 0;
        //    Status status;

        //    //fire off new requests
        //    for (int i = 0; i < maxAgents; i++)
        //    {
        //        if (!agents[i].IsActive)
        //            continue;
        //        if (agents[i].State == AgentState.Invalid)
        //            continue;
        //        if (agents[i].TargetState == TargetState.None || agents[i].TargetState == TargetState.Velocity)
        //            continue;

        //        if (agents[i].TargetState == TargetState.Requesting)
        //        {
        //            Path path = agents[i].Corridor.NavPath;

        //            Vector3 reqPos = new Vector3();
        //            Path reqPath = new Path();

        //            //quick search towards the goal
        //            const int MAX_ITER = 20;
        //            NavPoint startPoint = new NavPoint(path[0], agents[i].Position);
        //            NavPoint endPoint = new NavPoint(agents[i].TargetRef, agents[i].TargetPosition);
        //            navQuery.InitSlicedFindPath(ref startPoint, ref endPoint, navQueryFilter, FindPathOptions.None);
        //            int tempInt = 0;
        //            navQuery.UpdateSlicedFindPath(MAX_ITER, ref tempInt);
        //            status = Status.Failure;
        //            if (agents[i].TargetReplan)
        //            {
        //                //try to use an existing steady path during replan if possible
        //                status = navQuery.FinalizedSlicedPathPartial(path, reqPath).ToStatus();
        //            }
        //            else
        //            {
        //                //try to move towards the target when the goal changes
        //                status = navQuery.FinalizeSlicedFindPath(reqPath).ToStatus();
        //            }

        //            if (status != Status.Failure && reqPath.Count > 0)
        //            {
        //                //in progress or succeed
        //                if (reqPath[reqPath.Count - 1] != agents[i].TargetRef)
        //                {
        //                    //partial path, constrain target position in last polygon
        //                    bool tempBool;
        //                    status = navQuery.ClosestPointOnPoly(reqPath[reqPath.Count - 1], agents[i].TargetPosition, out reqPos, out tempBool).ToStatus();
        //                    if (status == Status.Failure)
        //                        reqPath.Clear();
        //                }
        //                else
        //                {
        //                    reqPos = agents[i].TargetPosition;
        //                }
        //            }
        //            else
        //            {
        //                reqPath.Clear();
        //            }

        //            if (reqPath.Count == 0)
        //            {
        //                //could not find path, start the request from the current location
        //                reqPos = agents[i].Position;
        //                reqPath.Add(path[0]);
        //            }

        //            agents[i].Corridor.SetCorridor(reqPos, reqPath);
        //            agents[i].Boundary.Reset();
        //            agents[i].IsPartial = false;

        //            if (reqPath[reqPath.Count - 1] == agents[i].TargetRef)
        //            {
        //                agents[i].TargetState = TargetState.Valid;
        //                agents[i].TargetReplanTime = 0.0f;
        //            }
        //            else
        //            {
        //                //the path is longer or potentially unreachable, full plan
        //                agents[i].TargetState = TargetState.WaitingForQueue;
        //            }
        //        }

        //        if (agents[i].TargetState == TargetState.WaitingForQueue)
        //        {
        //            numQueue = AddToPathQueue(agents[i], queue, numQueue, PATH_MAX_AGENTS);
        //        }
        //    }

        //    for (int i = 0; i < numQueue; i++)
        //    {
        //        queue[i].TargetPathQueryIndex = pathq.Request(new NavPoint(queue[i].Corridor.GetLastPoly(), queue[i].Corridor.Target), new NavPoint(queue[i].TargetRef, queue[i].TargetPosition));
        //        if (queue[i].TargetPathQueryIndex != PathQueue.Invalid)
        //            queue[i].TargetState = TargetState.WaitingForPath;
        //    }

        //    //update requests
        //    pathq.Update(MaxItersPerUpdate);

        //    //process path results
        //    for (int i = 0; i < maxAgents; i++)
        //    {
        //        if (!agents[i].IsActive)
        //            continue;
        //        if (agents[i].TargetState == TargetState.None || agents[i].TargetState == TargetState.Velocity)
        //            continue;

        //        if (agents[i].TargetState == TargetState.WaitingForPath)
        //        {
        //            //poll path queue
        //            status = pathq.GetRequestStatus(agents[i].TargetPathQueryIndex);
        //            if (status == Status.Failure)
        //            {
        //                //path find failed, retry if the target location is still valid
        //                agents[i].TargetPathQueryIndex = PathQueue.Invalid;
        //                if (agents[i].TargetRef != NavPolyId.Null)
        //                    agents[i].TargetState = TargetState.Requesting;
        //                else
        //                    agents[i].TargetState = TargetState.Failed;
        //                agents[i].TargetReplanTime = 0.0f;
        //            }
        //            else if (status == Status.Success)
        //            {
        //                Path path = agents[i].Corridor.NavPath;

        //                //apply results
        //                Vector3 targetPos = new Vector3();
        //                targetPos = agents[i].TargetPosition;

        //                Path res;
        //                bool valid = true;
        //                status = pathq.GetPathResult(agents[i].TargetPathQueryIndex, out res).ToStatus();
        //                if (status == Status.Failure || res.Count == 0)
        //                    valid = false;

        //                //Merge result and existing path
        //                if (valid && path[path.Count - 1] != res[0])
        //                    valid = false;

        //                if (valid)
        //                {
        //                    //put the old path infront of the old path
        //                    if (path.Count > 1)
        //                    {
        //                        //make space for the old path
        //                        //if ((path.Count - 1) + nres > maxPathResult)
        //                        //nres = maxPathResult - (npath - 1);

        //                        for (int j = 0; j < res.Count; j++)
        //                            res[path.Count - 1 + j] = res[j];

        //                        //copy old path in the beginning
        //                        for (int j = 0; j < path.Count - 1; j++)
        //                            res.Add(path[j]);

        //                        //remove trackbacks
        //                        res.RemoveTrackbacks();
        //                    }

        //                    //check for partial path
        //                    if (res[res.Count - 1] != agents[i].TargetRef)
        //                    {
        //                        //partial path, constrain target position inside the last polygon
        //                        Vector3 nearest;
        //                        bool tempBool = false;
        //                        status = navQuery.ClosestPointOnPoly(res[res.Count - 1], targetPos, out nearest, out tempBool).ToStatus();
        //                        if (status == Status.Success)
        //                            targetPos = nearest;
        //                        else
        //                            valid = false;
        //                    }
        //                }

        //                if (valid)
        //                {
        //                    //set current corridor
        //                    agents[i].Corridor.SetCorridor(targetPos, res);

        //                    //forced to update boundary
        //                    agents[i].Boundary.Reset();
        //                    agents[i].TargetState = TargetState.Valid;
        //                }
        //                else
        //                {
        //                    //something went wrong
        //                    agents[i].TargetState = TargetState.Failed;
        //                }

        //                agents[i].TargetReplanTime = 0.0f;
        //            }
        //        }
        //    }
        //}
    }
}
