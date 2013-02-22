﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Drydock.Logic.DoodadEditorState.Tools{
    internal class WallBuildTool : WallEditTool{
        readonly ObjectBuffer<ObjectIdentifier> _tempWallBuffer;
        readonly List<ObjectIdentifier> _tempWallIdentifiers;


        public WallBuildTool(HullDataManager hullData) :
            base(hullData){
            _tempWallBuffer = new ObjectBuffer<ObjectIdentifier>(
                hullData.DeckVertexes[0].Count()*2,
                10,
                20,
                30,
                "InternalWallTex")
                              {UpdateBufferManually = true};

            _tempWallIdentifiers = new List<ObjectIdentifier>();
        }

        protected override void HandleCursorChange(){
            GenerateWallsFromStroke();
        }

        protected override void HandleCursorEnd(){
            HullData.CurWallIdentifiers.AddRange(
                from id in _tempWallIdentifiers
                where !HullData.CurWallIdentifiers.Contains(id)
                select id
                );
            _tempWallIdentifiers.Clear();
            HullData.CurWallBuffer.AbsorbBuffer(_tempWallBuffer);
        }

        protected override void HandleCursorBegin(){
        }

        protected override void OnVisibleDeckChange(){
        }

        protected override void OnEnable(){
            _tempWallBuffer.Enabled = true;
        }

        protected override void OnDisable(){
            _tempWallBuffer.Enabled = false;
        }

        void GenerateWallsFromStroke(){
            _tempWallIdentifiers.Clear();
            int strokeW = (int) ((StrokeEnd.Z - StrokeOrigin.Z)/WallResolution);
            int strokeH = (int) ((StrokeEnd.X - StrokeOrigin.X)/WallResolution);

            _tempWallBuffer.ClearObjects();
            int wDir;
            int hDir;
            if (strokeW > 0)
                wDir = 1;
            else
                wDir = -1;
            if (strokeH > 0)
                hDir = 1;
            else
                hDir = -1;

            //generate width walls
            const float wallWidth = 0.1f;
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution*wDir);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeW); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeEnd.X, StrokeOrigin.Y, StrokeOrigin.Z + WallResolution*i*wDir);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, wallWidth, WallHeight, WallResolution*wDir);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X, origin.Y, origin.Z + WallResolution*wDir));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            //generate height walls
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeOrigin.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, WallHeight, wallWidth);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }
            for (int i = 0; i < Math.Abs(strokeH); i++){
                int[] indicies;
                VertexPositionNormalTexture[] verticies;
                var origin = new Vector3(StrokeOrigin.X + WallResolution*i*hDir, StrokeOrigin.Y, StrokeEnd.Z);
                MeshHelper.GenerateCube(out verticies, out indicies, origin, WallResolution*hDir, WallHeight, wallWidth);
                var identifier = new ObjectIdentifier(origin, new Vector3(origin.X + WallResolution*hDir, origin.Y, origin.Z));
                _tempWallBuffer.AddObject(identifier, indicies, verticies);
                _tempWallIdentifiers.Add(identifier);
            }

            _tempWallBuffer.UpdateBuffers();
        }
    }
}