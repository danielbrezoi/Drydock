﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Drydock.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Color = System.Drawing.Color;

namespace Drydock.Logic.DoodadEditorState {
    static class AirshipPackager {
        const int _version = 0;

        static public void Export(string fileName, HullDataManager hullData){
            JObject jObj = new JObject();
            jObj["Version"] = _version;
            jObj["NumDecks"] = hullData.NumDecks;

            var hullInds = new int[hullData.NumDecks][];
            var hullVerts = new VertexPositionNormalTexture[hullData.NumDecks][];

            for (int i = 0; i < hullData.NumDecks; i++){
                hullInds[i] = hullData.HullBuffers[i].DumpIndicies();
                hullVerts[i] = hullData.HullBuffers[i].DumpVerticies();
            }

            jObj["HullVerticies"] = JToken.FromObject(hullVerts);
            jObj["HullIndicies"] = JToken.FromObject(hullInds);

            var deckPlateInds = new List<int>[hullData.NumDecks];
            var deckPlateVerts = new List<VertexPositionNormalTexture>[hullData.NumDecks];

            for (int i = 0; i < hullData.NumDecks; i++){
                ConcatDeckPlates(
                    hullData.DeckBuffers[0].DumpObjectData(),
                    0.5f,
                    out deckPlateInds[i],
                    out deckPlateVerts[i]
                    );
            }

            jObj["DeckVerticies"] = JToken.FromObject(deckPlateVerts);
            jObj["DeckIndicies"] = JToken.FromObject(deckPlateInds);

            var sw = new StreamWriter(Directory.GetCurrentDirectory()+"\\Data\\"+fileName);
            sw.Write(JsonConvert.SerializeObject(jObj, Formatting.Indented));
            sw.Close();
        }

        static void ConcatDeckPlates(
            ObjectBuffer<ObjectIdentifier>.ObjectData[] objectData,
            float deckPlateWidth,
            out List<int> indicies,
            out List<VertexPositionNormalTexture> verticies){

            //this identifies deck boards that aren't part of the main mesh
            var nullIdentifier = new ObjectIdentifier(ObjectType.Misc, Vector3.Zero); 

            //get extrema
            float minX = float.MaxValue, minZ = float.MaxValue, maxX = 0, maxZ = 0;
            foreach (var data in objectData){
                if (data.Identifier.Equals(nullIdentifier))
                    continue;

                foreach (var point in data.Verticies){
                    if (point.Position.X > maxX)
                        maxX = point.Position.X;
                    if (point.Position.Z > maxZ)
                        maxZ = point.Position.Z;
                    if (point.Position.X < minX)
                        minX = point.Position.X;
                    if (point.Position.Z < minZ)
                        minZ = point.Position.Z;
                }
            }
            float y = objectData[0].Verticies[0].Position.Y;
            float mult = 1 / deckPlateWidth;

            Func<float, int> toArrX = f => (int)((f - minX) * mult);
            Func<float, int> toArrZ = f => (int)((f - minZ) * mult);

            Func<int, float> fromArrX = f => (float)f / mult + minX;
            Func<int, float> fromArrZ = f => (float)f / mult + minZ;

            var vertArr = new bool[toArrX(maxX)+1, toArrZ(maxZ)+1];
            var disabledVerts = new List<Tuple<int,int>>();
            foreach (var data in objectData){
                if (data.Identifier.Equals(nullIdentifier))
                    continue;

                if (data.Enabled) {
                    foreach (var vertex in data.Verticies) {
                        int x = toArrX(vertex.Position.X);
                        int z = toArrZ(vertex.Position.Z);
                        vertArr[x,z] = true;
                    }
                }
                else{
                    foreach (var vertex in data.Verticies) {
                        disabledVerts.Add(new Tuple<int, int>(
                            toArrX(vertex.Position.X),
                            toArrZ(vertex.Position.Z)
                            ));
                    }
                }
            }

            var listInds = new List<int>();
            var listVerts = new List<VertexPositionNormalTexture>();
            int numTiles = 0;
            var idxWinding = new[] { 0, 2, 1, 0, 3, 2};
            for (int xIdx = 0; xIdx < vertArr.GetLength(0) - 1; xIdx++) {
                int zIdx=0;

                while (true){
                    while (!vertArr[xIdx, zIdx] || !vertArr[xIdx+1, zIdx])
                        zIdx++;

                    int initlZ=zIdx;

                    while ((vertArr[xIdx, zIdx] && vertArr[xIdx + 1, zIdx])){
                        zIdx++;
                        if (zIdx + 1 > vertArr.GetLength(1))
                            break;
                    }

                    Func<int, int, int, int, bool> addVertex = (x, z, texU, texV) => {
                        listVerts.Add(new VertexPositionNormalTexture(
                                          new Vector3(
                                              fromArrX(x),
                                              y,
                                              fromArrZ(z)
                                              ),
                                          Vector3.Up,
                                          new Vector2(texU, texV)
                                          )
                            );
                        return true;
                    };

                    zIdx--;
                    addVertex(xIdx, initlZ, 0, 0);
                    addVertex(xIdx, zIdx, 1, 0);
                    addVertex(xIdx + 1, zIdx, 1, 1);
                    addVertex(xIdx + 1, initlZ, 0, 1);
                    int offset = numTiles * 4;

                    var winding = (int[])idxWinding.Clone();
                    for (int i = 0; i < 6; i++) {
                        winding[i] += offset;
                    }
                    listInds.AddRange(winding);
                    numTiles++;

                    //xxxx untested
                    if (!disabledVerts.Contains(new Tuple<int, int>(xIdx, zIdx))){
                        break;
                    }
                }
            }

            /*
            var bmp = new Bitmap(vertArr.GetLength(0), vertArr.GetLength(1));
            for (int x = 0; x < vertArr.GetLength(0); x++){
                for (int z = 0; z < vertArr.GetLength(1); z++){
                    if (vertArr[x, z]){
                        bmp.SetPixel(x, z, Color.Red);
                    }
                }
            }
            bmp.Save("hello.png");
            */

            indicies = listInds;
            verticies = listVerts;
        }

        static public void Import() {
            var sw = new Stopwatch();
            sw.Start();
            var sr = new StreamReader(Directory.GetCurrentDirectory() + "\\Data\\" + "Export.aship");
            var v = JsonConvert.DeserializeObject(sr.ReadToEnd());
            int fd = 3;

            sw.Stop();
            double d = sw.ElapsedMilliseconds;
            int dd = 3;
        }
    }
}
