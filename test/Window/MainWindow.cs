using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenTK;
using OpenTK.Graphics;
//using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using Gwen;
using Simulate;
using Key = OpenTK.Input.Key;
using SharpNav;
using SharpNav.Pathfinding;
using Simulate;
using RVO;
using SharpNav.Geometry;

namespace Window
{
    public sealed class MainWindow : GameWindow
    {
        Camera cam;
        private float zoom = OpenTK.MathHelper.PiOver4;
        AgentCylinder agentCylinder;


        private KeyboardState prevK;
        private MouseState prevM;

        private Gwen.Input.OpenTK gwenInput;
        private Gwen.Renderer.OpenTK gwenRenderer;
        private Gwen.Skin.Base gwenSkin;
        private Gwen.Control.Canvas gwenCanvas;
        private Matrix4 gwenProjection;

        private void InitializeOpenGL()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(Color4.White);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Light(LightName.Light0, LightParameter.Ambient, new Vector4(0.6f, 0.6f, 0.6f, 1f));
            GL.Disable(EnableCap.Light0);
            GL.Disable(EnableCap.Lighting);

            agentCylinder = new AgentCylinder(12, 0.2f, 1f);
        }


        public MainWindow()
        : base(1280, // initial width
            720, // initial height
            GraphicsMode.Default,
            "dreamstatecoding",  // initial title
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4, // OpenGL major version
            0, // OpenGL minor version
            GraphicsContextFlags.ForwardCompatible)

        {
            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);



            cam = new Camera();

            Keyboard.KeyDown += OnKeyboardKeyDown;
            Keyboard.KeyUp += OnKeyboardKeyUp;
            Mouse.ButtonDown += OnMouseButtonDown;
            Mouse.ButtonUp += OnMouseButtonUp;
            Mouse.Move += OnMouseMove;
            Mouse.WheelChanged += OnMouseWheel;

            this.Title = "song";

            Program.SongMain();
            //Program.Repeat(Program.UpdatePosition, 10);

        }



        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeOpenGL();

            //LoadLevel();
            //LoadDebugMeshes();

            gwenRenderer = new Gwen.Renderer.OpenTK();
            gwenSkin = new Gwen.Skin.TexturedBase(gwenRenderer, "GwenSkin.png");
            gwenCanvas = new Gwen.Control.Canvas(gwenSkin);
            gwenInput = new Gwen.Input.OpenTK(this);

            gwenInput.Initialize(gwenCanvas);
            gwenCanvas.SetSize(Width, Height);
            gwenCanvas.ShouldDrawBackground = false;

            gwenProjection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1, 1);



            //InitializeUI();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);


            if (!Focused)
                return;

            KeyboardState k = OpenTK.Input.Keyboard.GetState();
            MouseState m = OpenTK.Input.Mouse.GetState();

            bool isShiftDown = false;
            if (k[Key.LShift] || k[Key.RShift])
                isShiftDown = true;

            //TODO make cam speed/shift speedup controllable from GUI
            float camSpeed = 80f * (float)e.Time * (isShiftDown ? 3f : 1f);
            float zoomSpeed = (float)Math.PI * (float)e.Time * (isShiftDown ? 0.2f : 0.1f);

            if (k[Key.W])
                cam.Move(-camSpeed);
            if (k[Key.A])
                cam.Strafe(-camSpeed);
            if (k[Key.S])
                cam.Move(camSpeed);
            if (k[Key.D])
                cam.Strafe(camSpeed);
            if (k[Key.Q])
                cam.Elevate(camSpeed);
            if (k[Key.E])
                cam.Elevate(-camSpeed);
            if (k[Key.Z])
            {
                zoom += zoomSpeed;
                if (zoom > OpenTK.MathHelper.PiOver2)
                    zoom = OpenTK.MathHelper.PiOver2;
            }
            if (k[Key.C])
            {
                zoom -= zoomSpeed;
                if (zoom < 0.002f)
                    zoom = 0.002f;
            }

            if (m[MouseButton.Right])
            {
                cam.RotatePitch((m.X - prevM.X) * (float)e.Time * 2f);
                cam.RotateHeading((prevM.Y - m.Y) * (float)e.Time * 2f);
            }

            float aspect = Width / (float)Height;
            Matrix4 persp = Matrix4.CreatePerspectiveFieldOfView(zoom, aspect, 0.1f, 1000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref persp);
            GL.MatrixMode(MatrixMode.Modelview);
            cam.LoadView();
            //if (crowd != null)
            //    crowd.Update((float)e.Time);

            prevK = k;
            prevM = m;

            if (gwenRenderer.TextCacheSize > 1000)
                gwenRenderer.FlushTextCache();
        }

        protected void OnKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!Focused)
                return;

            if (e.Key == Key.Escape)
                Exit();
            //else if (e.Key == Key.F5)
            //    Gwen.Platform.Neutral.FileSave("Save NavMesh to file", ".", "All SharpNav Files(.snb, .snx, .snj)|*.snb;*.snx;*.snj|SharpNav Binary(.snb)|*.snb|SharpNav XML(.snx)|*.snx|SharpNav JSON(.snj)|*.snj", SaveNavMeshToFile);
            //else if (e.Key == Key.F9)
            //    Gwen.Platform.Neutral.FileOpen("Load NavMesh from file", ".", "All SharpNav Files(.snb, .snx, .snj)|*.snb;*.snx;*.snj|SharpNav Binary(.snb)|*.snb|SharpNav XML(.snx)|*.snx|SharpNav JSON(.snj)|*.snj", LoadNavMeshFromFile);
            else if (e.Key == Key.F11)
                WindowState = OpenTK.WindowState.Normal;
            else if (e.Key == Key.F12)
                WindowState = OpenTK.WindowState.Fullscreen;

            gwenInput.ProcessKeyDown(e);

            base.OnKeyDown(e);
        }

        protected void OnKeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (!Focused)
                return;

            gwenInput.ProcessKeyUp(e);
        }



        private static int[,] board = new int[3, 3];        //Definition
        private const int BUFSIZE = 512;

        protected void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Focused)
                return;


            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Ortho(-10, 10, -10, 10, -10, 10);

            //GLfloat m[16];
            //glGetFloatv(GL_PROJECTION_MATRIX, m);

            int[] selectBuffer = new int[BUFSIZE];              //This has to be redifined
            int hits;
            int[] viewport = new int[4];

            if (e.Button == MouseButton.Left)
            {
                GL.GetInteger(GetPName.Viewport, viewport);
                GL.SelectBuffer(BUFSIZE, selectBuffer);
                GL.RenderMode(RenderingMode.Select);
                GL.InitNames();
                GL.PushName(0);
                GL.MatrixMode(MatrixMode.Projection);
                GL.PushMatrix();
                GL.LoadIdentity();
                UIntPtr Pixel = new UIntPtr();
                GL.ReadPixels(e.X, viewport[3] - e.Y, 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref Pixel);
                uint SelectedTriangle = SelectedTriangle = Pixel.ToUInt32();
                GL.Ortho(0, 3, 0, 3, 1, -1); // Bottom-left corner pixel has coordinate (0, 0)                 
                DrawSquares(GL.RenderMode(RenderingMode.Select));
                GL.MatrixMode(MatrixMode.Projection);
                GL.PopMatrix();
                GL.Flush();
                hits = GL.RenderMode(RenderingMode.Render);
                ProcessHits(hits, selectBuffer);
                SwapBuffers();
            }



            gwenInput.ProcessMouseMessage(e);
        }

        private void ProcessHits(int hits, int[] selectBuffer)
        {
            //throw new NotImplementedException();
        }

        private static void DrawSquares(int mode)
        {
            int i, j;
            for (i = 0; i < 3; i++)
            {
                if (mode == GL.RenderMode(RenderingMode.Select))
                    GL.LoadName(i);
                for (j = 0; j < 3; j++)
                {
                    if (mode == GL.RenderMode(RenderingMode.Select))
                        GL.PushName(j);
                    GL.Color3((float)i / 3.0f, (float)j / 3.0f, (float)board[i, j] / 3.0f);
                    GL.Rect(i, j, (i + 1), (j + 1));
                    if (mode == GL.RenderMode(RenderingMode.Select))
                        GL.PopName();
                }
            }
        }

        protected void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!Focused)
                return;

            gwenInput.ProcessMouseMessage(e);
        }

        protected void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            gwenInput.ProcessMouseMessage(e);
        }

        protected void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Focused)
                return;

            gwenInput.ProcessMouseMessage(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

#if showmesh || read 
            DrawNavMesh();
#endif
            DrawPolyMeshDetail();


            DrawCrowd();
            //DrawUI();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
            float aspect = Width / (float)Height;

            Matrix4 persp = Matrix4.CreatePerspectiveFieldOfView(zoom, aspect, 0.1f, 1000f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref persp);
            GL.MatrixMode(MatrixMode.Modelview);
            cam.LoadView();

            gwenProjection = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, 0, 1);
            gwenCanvas.SetSize(Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            gwenCanvas.Dispose();
            gwenSkin.Dispose();
            gwenRenderer.Dispose();

            //UnloadLevel();
            //UnloadDebugMeshes();

            base.OnUnload(e);
        }

        private void DrawPolyMeshDetail()
        {
            if (Program.nav.polyMeshDetail == null)
                return;

            GL.PushMatrix();

            Matrix4 transMatrix = Matrix4.CreateTranslation(0, -Program.nav.polyMesh.CellHeight, 0);
            GL.MultMatrix(ref transMatrix);

            Color4 color = Color4.DarkViolet;
            color.A = 0.5f;
            GL.Color4(color);

            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < Program.nav.polyMeshDetail.MeshCount; i++)
            {

                PolyMeshDetail.MeshData m = Program.nav.polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {
                    if (color.R < 1) color.R += 0.1f;
                    if (color.G < 1) color.G += 0.1f;
                    if (color.R >= 1) color.R = 0.1f;
                    if (color.G >= 1) color.G = 0.1f;
                    GL.Color4(color);
                    var t = Program.nav.polyMeshDetail.Tris[triIndex + j];

                    var v = Program.nav.polyMeshDetail.Verts[vertIndex + t.VertexHash0];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = Program.nav.polyMeshDetail.Verts[vertIndex + t.VertexHash1];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = Program.nav.polyMeshDetail.Verts[vertIndex + t.VertexHash2];
                    GL.Vertex3(v.X, v.Y, v.Z);
                }

            }
            GL.End();


            GL.Color4(Color4.Purple);
            GL.LineWidth(0.5f);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < Program.nav.polyMeshDetail.MeshCount; i++)
            {
                var m = Program.nav.polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {
                    var t = Program.nav.polyMeshDetail.Tris[triIndex + j];
                    for (int k = 0, kp = 2; k < 3; kp = k++)
                    {
                        if (((t.Flags >> (kp * 2)) & 0x3) == 0)
                        {
                            if (t[kp] < t[k])
                            {
                                var v = Program.nav.polyMeshDetail.Verts[vertIndex + t[kp]];
                                GL.Vertex3(v.X, v.Y, v.Z);

                                v = Program.nav.polyMeshDetail.Verts[vertIndex + t[k]];
                                GL.Vertex3(v.X, v.Y, v.Z);
                            }
                        }
                    }
                }
            }

            GL.End();

            GL.Color4(Color4.Red);
            GL.LineWidth(0.5f);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < Program.nav.polyMeshDetail.MeshCount; i++)
            {
                var m = Program.nav.polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {
                    var t = Program.nav.polyMeshDetail.Tris[triIndex + j];
                    for (int k = 0, kp = 2; k < 3; kp = k++)
                    {
                        if (((t.Flags >> (kp * 2)) & 0x3) == 0)
                        {
                            if (t[kp] < t[k])
                            {
                                var v = Program.nav.polyMeshDetail.Verts[vertIndex + t[kp]];
                                GL.Vertex3(v.X, v.Y, v.Z);
                                
                                v = Program.nav.polyMeshDetail.Verts[vertIndex + t[k]];
                                GL.Vertex3(v.X, v.Y, v.Z);
                            }
                        }
                    }
                }
            }

            //GL.Vertex3(-144, 2, -372);
            //GL.Vertex3(-280, 2,124.8);
            GL.End();

            GL.LineWidth(0.5f);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < Program.nav.polyMeshDetail.MeshCount; i++)
            {
                var m = Program.nav.polyMeshDetail.Meshes[i];

                int vertIndex = m.VertexIndex;
                int triIndex = m.TriangleIndex;

                for (int j = 0; j < m.TriangleCount; j++)
                {
                    var t = Program.nav.polyMeshDetail.Tris[triIndex + j];
                    for (int k = 0, kp = 2; k < 3; kp = k++)
                    {
                        if (((t.Flags >> (kp * 2)) & 0x3) != 0)
                        {
                            var v = Program.nav.polyMeshDetail.Verts[vertIndex + t[kp]];
                            GL.Vertex3(v.X, v.Y, v.Z);

                            v = Program.nav.polyMeshDetail.Verts[vertIndex + t[k]];
                            GL.Vertex3(v.X, v.Y, v.Z);
                        }
                    }
                }
            }

            GL.End();

            //         GL.PointSize(4.8f);
            //GL.Begin(BeginMode.Points);
            //for (int i = 0; i < polyMeshDetail.MeshCount; i++)
            //{
            //	var m = polyMeshDetail.Meshes[i];

            //	for (int j = 0; j < m.VertexCount; j++)
            //	{
            //		var v = polyMeshDetail.Verts[m.VertexIndex + j];
            //		GL.Vertex3(v.X, v.Y, v.Z);
            //	}
            //}

            //GL.End();

            GL.PopMatrix();
        }
        private void DrawNavMesh()
        {
            if (Program.nav.tiledNavMesh == null)
                return;

            var tile = Program.nav.tiledNavMesh.GetTileAt(0, 0, 0);

            GL.PushMatrix();

            Color4 color = Color4.AntiqueWhite;
            color.A = 0.2f;
            GL.Color4(color);

            GL.Begin(BeginMode.Triangles);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    if (color.R < 1) color.R += 0.1f;
                    if (color.G < 1) color.G += 0.1f;
                    if (color.R >= 1) color.R = 0.1f;
                    if (color.G >= 1) color.G = 0.1f;
                    GL.Color4(color);

                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];

                    var v = tile.Verts[vertIndex0];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = tile.Verts[vertIndex1];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = tile.Verts[vertIndex2];
                    GL.Vertex3(v.X, v.Y, v.Z);
                }
            }

            GL.End();

            GL.DepthMask(false);

            //neighbor edges
            GL.Color4(Color4.Purple);

            GL.LineWidth(1f);
            GL.Begin(BeginMode.Lines);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    if (tile.Polys[i].Verts[j] == 0)
                        break;
                    if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
                        continue;

                    int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

                    int vertIndex0 = tile.Polys[i].Verts[j];
                    int vertIndex1 = tile.Polys[i].Verts[nj];

                    var v = tile.Verts[vertIndex0];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = tile.Verts[vertIndex1];
                    GL.Vertex3(v.X, v.Y, v.Z);
                }
            }

            GL.End();

            //boundary edges
            GL.LineWidth(2f);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < tile.Polys.Length; i++)
            {
                for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
                        continue;

                    int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

                    int vertIndex0 = tile.Polys[i].Verts[j];
                    int vertIndex1 = tile.Polys[i].Verts[nj];

                    var v = tile.Verts[vertIndex0];
                    GL.Vertex3(v.X, v.Y, v.Z);

                    v = tile.Verts[vertIndex1];
                    GL.Vertex3(v.X, v.Y, v.Z);
                }
            }

            GL.End();

            GL.PointSize(4.8f);
            GL.Begin(BeginMode.Points);

            //for (int i = 0; i < tile.Verts.Length; i++)
            //{
            //    var v = tile.Verts[i];
            //    GL.Vertex3(v.X, v.Y, v.Z);
            //}

            GL.End();

            GL.DepthMask(true);

            GL.PopMatrix();
        }

        public void DrawCrowd()
        {
            if (Program._agents.Count == 0)
                return;

            GL.PushMatrix();

            //The black line represents the actual path that the agent takes
            /*GL.Color4(Color4.Black);
			GL.Begin(BeginMode.Lines);
			for (int i = 0; i < numActiveAgents; i++)
			{
				for (int j = 0; j < numIterations - 1; j++)
				{
					SVector3 v0 = trails[i].Trail[j];
					GL.Vertex3(v0.X, v0.Y, v0.Z);

					SVector3 v1 = trails[i].Trail[j + 1];
					GL.Vertex3(v1.X, v1.Y, v1.Z);
				}
			}
			GL.End();

			//The yellow line represents the ideal path from the start to the target
			GL.Color4(Color4.Yellow);
			GL.LineWidth(1.5f);
			GL.Begin(BeginMode.Lines);
			for (int i = 0; i < numActiveAgents; i++)
			{
				SVector3 v0 = trails[i].Trail[0];
				GL.Vertex3(v0.X, v0.Y, v0.Z);

				SVector3 v1 = trails[i].Trail[AGENT_MAX_TRAIL - 1];
				GL.Vertex3(v1.X, v1.Y, v1.Z);
			}
			GL.End();

			//The cyan point represents the agent's starting location
			GL.PointSize(100.0f);
			GL.Color4(Color4.Cyan);
			GL.Begin(BeginMode.Points);
			for (int i = 0; i < numActiveAgents; i++)
			{
				SVector3 v0 = trails[i].Trail[0];
				GL.Vertex3(v0.X, v0.Y, v0.Z);
			}
			GL.End();

			//The red point represent's the agent's target location
			GL.Color4(Color4.PaleVioletRed);
			GL.Begin(BeginMode.Points);
			for (int i = 0; i < numActiveAgents; i++)
			{
				SVector3 v0 = trails[i].Trail[AGENT_MAX_TRAIL - 1];
				GL.Vertex3(v0.X, v0.Y, v0.Z);
			}
			GL.End();*/
            //GL.DepthMask(true);

            GL.Color4(Color4.PaleVioletRed);
            GL.PointSize(10);

            GL.LineWidth(0.8f);

            //for (int i = 0; i < Program._agents.Count; i++)
            //{
            //    GL.Begin(BeginMode.Lines);
            //    GL.Color4(Color4.Green);
            //    //GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()),0.5f));
            //    for (int j = 0; j < Program._agents[i].navPoints.Count - 1; j++)
            //    {
            //        SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Program._agents[i].navPoints[j].x_, 1f, Program._agents[i].navPoints[j].y_);
            //        GL.Vertex3(p.X, p.Y, p.Z);
            //    }
            //    GL.End();
            //}

          

            for (int i = 0; i < Program._agents.Count; i++)
            {
                //song 画导航点
                //GL.Begin(BeginMode.Points);
                //GL.Color4(Color4.Green);
                ////GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()),0.5f));
                //for (int j = 0; j < Program._agents[i].navPoints.Count - 1; j++)
                //{
                //    SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Program._agents[i].navPoints[j].x_, 1f, Program._agents[i].navPoints[j].y_);
                //    GL.Vertex3(p.X, p.Y, p.Z);
                //}
                //GL.End();

                ////画导航线 song
                //GL.Begin(BeginMode.Lines);
                //GL.Color4(Color4.Green);
                ////GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()),0.5f));
                //for (int j = 1; j < Program._agents[i].navPoints.Count - 1; j++)
                //{
                //    SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Program._agents[i].navPoints[j].x_, 1f, Program._agents[i].navPoints[j].y_);
                //    GL.Vertex3(p.X, p.Y, p.Z);
                //    p = new SharpNav.Geometry.Vector3(Program._agents[i].navPoints[j - 1].x_, 1f, Program._agents[i].navPoints[j - 1].y_);
                //    GL.Vertex3(p.X, p.Y, p.Z);
                //}
                //GL.End();

                //画当前目标线
                //try
                //{
                //    GL.Begin(BeginMode.Lines);
                //    GL.Color4(Program._agents[i].color);
                //    if (Program._agents[i].navPoints.Count > 1)
                //    {

                //        SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Program._agents[i].positionNow.x_, 1.01f, Program._agents[i].positionNow.y_);
                //        GL.Vertex3(p.X, p.Y, p.Z);
                //        p = new SharpNav.Geometry.Vector3(Program._agents[i].navPoints[0].x_, 1.01f, Program._agents[i].navPoints[0].y_);
                //        GL.Vertex3(p.X, p.Y, p.Z);
                //    }
                //    GL.End();
                //}
                //catch
                //{

                //}
       
            }



            GL.Color4(Color4.Green);
            if (agentCylinder != null)
            {
                
                for (int i = 0; i < Program._agents.Count; i++)
                {
                    try
                    {
                        SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Program._agents[i].positionNow.x_, 0, Program._agents[i].positionNow.y_);
                        agentCylinder.Draw(new OpenTK.Vector3(p.X, p.Y, p.Z),Program._agents[i].color);
//#if outfile    
//                        FileHelper.Write(Program._agents[i].positionNow.x_ + " " + Program._agents[i].positionNow.y_ + " ");
//#endif
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("多线程对_agents.count访问出错 - 绘制");
                        Console.WriteLine(e);
                    }
                    
                }
//#if outfile
//                FileHelper.NewLine();
//#endif

                //#if outfile
                //            //输出agent信息文件
                //            for (int i = 0; i < Program._agents.Count; i++)
                //            {
                //                //FileHelper.Write(_agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " " + _agents[i].navPoints.Count + " ");
                //                FileHelper.Write(Program._agents[i].positionNow.x_ + " " + Program._agents[i].positionNow.y_ + " ");
                //            }
                //            FileHelper.NewLine();
                //#endif

            }

            GL.PopMatrix();
        }
    }


}