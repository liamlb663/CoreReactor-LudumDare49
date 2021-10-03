using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoreReactor
{
    public struct LevelData
    {
        public string lvljson; //Acts as both json data and path to json file
        dynamic json;

        //TileData
        private Vector2 tileSize;
        public List<Wall> walls;

        //NonTileData
        public List<GameObject> objects;
        public Player player;
        public List<Rectangle> cameraBounds;
        public List<Rectangle> LevelTriggers;

        public LevelData(string Json)
        {
            //Set all lists to empty
            cameraBounds = new List<Rectangle>();
            LevelTriggers = new List<Rectangle>();
            objects = new List<GameObject>();
            walls = new List<Wall>();
            player = new Player(Vector2.Zero);

            #region Wall Tiles

            //Get and Deserialize lvljson and place into json dynamic
            using (var streamReader = new StreamReader(Path.GetFullPath(Json)))
            {
                lvljson = streamReader.ReadToEnd();
            }
            json = JsonConvert.DeserializeObject(lvljson);

            //Extract data from json

            tileSize.X = (float)json.SelectToken("tilewidth").ToObject<int>();
            tileSize.Y = (float)json.SelectToken("tileheight").ToObject<int>();

            //Get Chunk Data For Tiles
            JArray chunks = json.SelectToken("layers[0].chunks")?.ToObject<JArray>();

            Vector2 tiledOffset = Vector2.Zero;
            Vector2 offset = Vector2.Zero;
            tiledOffset.X += json.SelectToken("layers[0].x")?.ToObject<int>();
            tiledOffset.Y += json.SelectToken("layers[0].y")?.ToObject<int>();
            if (json.SelectToken("layers[0].offsetx")?.ToObject<int>() != null)
                tiledOffset.X += json.SelectToken("layers[0].offsetx")?.ToObject<int>();
            if (json.SelectToken("layers[0].offsety")?.ToObject<int>() != null)
                tiledOffset.Y += json.SelectToken("layers[0].offsety")?.ToObject<int>();


            if (chunks != null)
            {
                //Get data from chunks and assemble lists
                for (int i = 0; i < chunks.Count(); i++)//for each chunk
                {
                    //Get offset
                    offset.X = chunks[i].SelectToken("x").ToObject<int>();
                    offset.Y = chunks[i].SelectToken("y").ToObject<int>();

                    //Get width of chunk
                    int chunkWidth = chunks[i].SelectToken("width").ToObject<int>();

                    //Get data of chunk
                    int[] data = chunks[i].SelectToken("data").ToObject<int[]>();

                    //Get and assemble data from chunk[i]
                    for (int j = 0; j < data.Length; j++)
                    {
                        //Find Position on grid for each tile
                        Vector2 gridPos;
                        gridPos.Y = (int)(j / chunkWidth);
                        gridPos.X = (int)(j % chunkWidth);

                        //Find final position
                        Vector2 dataPosition;
                        dataPosition = (gridPos + offset) * tileSize;
                        dataPosition += tiledOffset;

                        //Assmeble depending on data point
                        int dataVal = data[j];
                        switch (dataVal)
                        {
                            case 1:
                                //Red
                                walls.Add(new Wall(dataPosition, tileSize));
                                break;
                            case 2:
                                //Black
                                walls.Add(new DieWall(dataPosition, tileSize));
                                break;
                            default:
                                break;
                        }

                    }

                }
            }

            #endregion

            #region Camera Bounds

            //Get Objects Data For CameraBounds
            JArray cameraBound = json.SelectToken("layers[1].objects")?.ToObject<JArray>();
            offset = Vector2.Zero;
            offset.X += json.SelectToken("layers[1].x")?.ToObject<int>();
            if (json.SelectToken("layers[1].offsetx")?.ToObject<int>() != null)
                offset.X += json.SelectToken("layers[1].offsetx")?.ToObject<int>();
            offset.Y += json.SelectToken("layers[1].y")?.ToObject<int>();
            if (json.SelectToken("layers[1].offsety")?.ToObject<int>() != null)
                offset.Y += json.SelectToken("layers[1].offsety")?.ToObject<int>();

            if (cameraBound != null)
            {
                //Get data from objects and assemble lists of cameraBounds
                for (int i = 0; i < cameraBound.Count(); i++)
                {
                    int x = cameraBound[i].SelectToken("x").ToObject<int>();
                    int y = cameraBound[i].SelectToken("y").ToObject<int>();
                    int width = cameraBound[i].SelectToken("width").ToObject<int>();
                    int height = cameraBound[i].SelectToken("height").ToObject<int>();

                    cameraBounds.Add(new Rectangle((int)(x + offset.X), (int)(y + offset.Y), width, height));
                }
            }

            #endregion

            #region Object Markers

            //Get Objects Data
            JArray Objects = json.SelectToken("layers[2].objects")?.ToObject<JArray>();
            offset = Vector2.Zero;
            offset.X += json.SelectToken("layers[2].x")?.ToObject<int>();
            if (json.SelectToken("layers[2].offsetx")?.ToObject<int>() != null)
                offset.X += json.SelectToken("layers[2].offsetx")?.ToObject<int>();
            offset.Y += json.SelectToken("layers[2].y")?.ToObject<int>();
            if (json.SelectToken("layers[2].offsety")?.ToObject<int>() != null)
                offset.Y += json.SelectToken("layers[2].offsety")?.ToObject<int>();

            if (Objects != null)
            {
                //Get data from objects and assemble lists of cameraBounds
                for (int i = 0; i < Objects.Count(); i++)
                {
                    int x = Objects[i].SelectToken("x").ToObject<int>();
                    int y = Objects[i].SelectToken("y").ToObject<int>();
                    string name = Objects[i].SelectToken("name").ToObject<string>().ToLower();

                    switch (name)
                    {

                        case "player":
                            player = new Player(new Vector2(x + offset.X, y + offset.Y));
                            break;

                        case "core":
                            objects.Add(new Core(new Vector2(x + offset.X, y + offset.Y)));
                            break;

                        case "holder":
                            objects.Add(new Holder(new Vector2(x + offset.X, y + offset.Y)));
                            break;

                        default:
                            throw new NotImplementedException("Object Not added to index");
                    }

                }
            }

            #endregion

            #region Level Triggers
            
            //Get Objects Data For Level Triggers
            JArray lvlTriggers = json.SelectToken("layers[3].objects")?.ToObject<JArray>();
            offset = Vector2.Zero;
            offset.X += json.SelectToken("layers[3].x")?.ToObject<int>();
            if (json.SelectToken("layers[3].offsetx")?.ToObject<int>() != null)
                offset.X += json.SelectToken("layers[3].offsetx")?.ToObject<int>();
            offset.Y += json.SelectToken("layers[3].y")?.ToObject<int>();
            if (json.SelectToken("layers[3].offsety")?.ToObject<int>() != null)
                offset.Y += json.SelectToken("layers[3].offsety")?.ToObject<int>();

            if (lvlTriggers != null)
            {
                //Get data from objects and assemble lists of Level Triggers
                for (int i = 0; i < lvlTriggers.Count(); i++)
                {
                    int x = lvlTriggers[i].SelectToken("x").ToObject<int>();
                    int y = lvlTriggers[i].SelectToken("y").ToObject<int>();
                    int width = lvlTriggers[i].SelectToken("width").ToObject<int>();
                    int height = lvlTriggers[i].SelectToken("height").ToObject<int>();

                    LevelTriggers.Add(new Rectangle((int)(x + offset.X), (int)(y + offset.Y), width, height));
                }
            }
            
            #endregion

        }
    }


}
