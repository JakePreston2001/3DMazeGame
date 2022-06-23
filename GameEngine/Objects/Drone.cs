using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenGL_Game.Components;
using OpenTK;
using OpenGL_Game.Scenes;
using System.IO;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Objects
{
    //inherits from the entity class
    class Drone : Entity
    {
        //sets up the entities name and position
        string name;
        Vector3 newPos;
        public Drone(string name) : base(name)
        {
            //sets its name
            this.name = name;
        }

        public void getNextGrid(char[,] map, Vector3 cameraPos)
        {
            //
            IComponent positionComponent = this.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });

            ComponentPosition currentPos = (ComponentPosition)positionComponent;

            newPos = new Vector3((float)Math.Round(currentPos.Position.X / 2), currentPos.Position.Y, (float)Math.Round(currentPos.Position.Z / 2));

            Vector3 updateX = new Vector3(newPos);
            Vector3 updateZ = new Vector3(newPos);

            cameraPos = new Vector3((float)Math.Round(cameraPos.X / 2), cameraPos.Y, (float)Math.Round(cameraPos.Z / 2));

           //sets the next position for the drone
            if (newPos.X > cameraPos.X)
            {
                updateX.X -= 1;
                SetNextPos(updateX, map);
            }
            else if (newPos.X < cameraPos.X)
            {
                updateX.X += 1;
                SetNextPos(updateX, map);
            }

            if (newPos.Z > cameraPos.Z)
            {
                updateZ.Z -= 1;
                SetNextPos(updateZ, map);
            }
            else if (newPos.Z < cameraPos.Z)
            {
                updateZ.Z += 1;
                SetNextPos(updateZ, map);
            }

        }
        //sets the next position
        protected bool SetNextPos(Vector3 pos, char[,] map)
        {
            //checks to see if the drone is in a valid position
            if (pos.X < (newPos.X - 1)) return false;
            if (pos.X > (newPos.X + 1)) return false;
            if (pos.Z < (newPos.Z - 1)) return false;
            if (pos.Z > (newPos.Z + 1)) return false;
            if (!ValidPos(pos, map)) return false;
            GameScene.gameInstance.droneTarget = pos * 2;
            return true;
        }
        //checks to see if its a valid position - the drone does not care though
        public bool ValidPos(Vector3 pos, char[,] map)
        {
            if (pos.X < 0) return false;
            if (pos.X >= map.GetLength(0)) return false;
            if (pos.Z < 0) return false;
            if (pos.Z >= map.GetLength(0)) return false;
            if (map[(int)pos.X, (int)pos.Z] != 'X') { return true; }
            return true;
        }
    }
}
