﻿#region

using System;
using System.Linq;
using Drydock.Control;
using Drydock.Render;
using Drydock.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.HullEditorState {
    internal class PreviewRenderer : BodyCenteredCamera {
        const int _meshVertexWidth = 64; //this is in primitives
        readonly BezierCurveCollection _backCurves;
        readonly GeometryBuffer<VertexPositionNormalTexture> _geometryBuffer;
        readonly int[] _indicies;
        readonly Vector3[,] _mesh;
        readonly RenderTarget _renderTarget;
        readonly BezierCurveCollection _sideCurves;
        readonly BezierCurveCollection _topCurves;
        VertexPositionNormalTexture[] _verticies;

        public PreviewRenderer(BezierCurveCollection sideCurves, BezierCurveCollection topCurves, BezierCurveCollection backCurves) :
            base(new Rectangle(
                     ScreenData.GetScreenValueX(0.5f),
                     ScreenData.GetScreenValueY(0.5f),
                     ScreenData.GetScreenValueX(0.5f),
                     ScreenData.GetScreenValueY(0.5f)
                     )) {

            _renderTarget = new RenderTarget(
                ScreenData.GetScreenValueX(0.5f),
                ScreenData.GetScreenValueY(0.5f),
                ScreenData.GetScreenValueX(0.5f),
                ScreenData.GetScreenValueY(0.5f)
                );
            _renderTarget.Bind();
            _indicies = MeshHelper.CreateIndiceArray((_meshVertexWidth) * (_meshVertexWidth));
            _verticies = MeshHelper.CreateTexcoordedVertexList((_meshVertexWidth) * (_meshVertexWidth));

            _geometryBuffer = new GeometryBuffer<VertexPositionNormalTexture>(
                _indicies.Count(),
                _verticies.Count(),
                (_meshVertexWidth) * (_meshVertexWidth) * 2,
                "HullEffect"
            );

            _mesh = new Vector3[_meshVertexWidth, _meshVertexWidth];

            _sideCurves = sideCurves;
            _topCurves = topCurves;
            _backCurves = backCurves;

            _geometryBuffer.IndexBuffer.SetData(_indicies);
            _renderTarget.Unbind();
        }


        public void Draw() {
            Gbl.Device.Clear(Color.CornflowerBlue);
            var viewMatrix = Matrix.CreateLookAt(base.CameraPosition, base.CameraTarget, Vector3.Up);
            _renderTarget.Draw(viewMatrix, Color.CornflowerBlue);
        }

        public void Dispose(){
            _renderTarget.Dispose();
        }

        public void Update(ref InputState state) {
            base.UpdateInput(ref state);
            _topCurves.GetParameterizedPoint(0, true);

            var topPts = new Vector2[_meshVertexWidth];
            for (double i = 0; i < _meshVertexWidth; i++) {
                double t = i / ((_meshVertexWidth - 1) * 2);
                topPts[(int)i] = _topCurves.GetParameterizedPoint(t);
            }


            topPts = topPts.Reverse().ToArray();
            float reflectionPoint = topPts[0].Y;
            var yDelta = new float[_meshVertexWidth];

            for (int i = 0; i < _meshVertexWidth; i++) {
                yDelta[i] = Math.Abs(topPts[i].Y - reflectionPoint) * 2 / _meshVertexWidth;
            }


            _sideCurves.GetParameterizedPoint(0, true); //this refreshes internal fields
            //orient controllers correctly for the bezierintersect
            var li = _sideCurves.Select(bezierCurve => new BezierInfo(
                                                           _sideCurves.ToMeters(bezierCurve.CenterHandlePos),
                                                           _sideCurves.ToMeters(bezierCurve.PrevHandlePos),
                                                           _sideCurves.ToMeters(bezierCurve.NextHandlePos))).ToList();


            var sideIntersectGenerator = new BezierDependentGenerator(li);

            var sideIntersectionCache = new float[_meshVertexWidth];

            for (int x = 0; x < _meshVertexWidth; x++) {
                sideIntersectionCache[x] = sideIntersectGenerator.GetValueFromIndependent(topPts[x].X).Y;
            }

            var maxY = (float)_backCurves.ToMetersY(_backCurves.MaxY);
            var backCurvesMaxWidth = (float)(_backCurves.ToMetersX(_backCurves[_backCurves.Count - 1].CenterHandlePos.X) - _backCurves.ToMetersX(_backCurves[0].CenterHandlePos.X));

            for (int x = 0; x < _meshVertexWidth; x++) {
                float scaleX = Math.Abs((reflectionPoint - topPts[x].Y) * 2) / backCurvesMaxWidth;
                float scaleY = sideIntersectionCache[x] / maxY;

                var bezierInfo = _backCurves.GetControllerInfo(scaleX, scaleY);
                var crossIntersectGenerator = new BezierDependentGenerator(bezierInfo);

                for (int z = 0; z < _meshVertexWidth / 2; z++) {
                    Vector2 pos = crossIntersectGenerator.GetValueFromIndependent(yDelta[x] * (z));
                    _mesh[x, z] = new Vector3(topPts[x].X, -pos.Y, topPts[x].Y + yDelta[x] * (z));
                    _mesh[x, _meshVertexWidth - 1 - z] = new Vector3(topPts[x].X, -pos.Y, topPts[x].Y + yDelta[x] * (_meshVertexWidth - 1 - z));
                }
            }
            var normals = new Vector3[_meshVertexWidth, _meshVertexWidth];

            MeshHelper.GenerateMeshNormals(_mesh, ref normals);
            MeshHelper.ConvertMeshToVertList(_mesh, normals, ref _verticies);

            _geometryBuffer.VertexBuffer.SetData(_verticies);

            var p = new Vector3();
            p += _mesh[0, 0];
            p += _mesh[_meshVertexWidth - 1, 0];
            p += _mesh[0, _meshVertexWidth - 1];
            p += _mesh[_meshVertexWidth - 1, _meshVertexWidth - 1];
            p /= 4;
            base.SetCameraTarget(p);
        }

        //public void Dispose(){
        //_geometryBuffer.Dispose();
        //}
    }
}