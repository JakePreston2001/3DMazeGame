using System.Collections.Generic;
using OpenGL_Game.Objects;
using System.Diagnostics;
using OpenGL_Game.Components;

namespace OpenGL_Game.Managers
{
    class EntityManager
    {
        List<Entity> entityList;
        public EntityManager()
        {
            entityList = new List<Entity>();
        }

        public void AddEntity(Entity entity)
        {
            Entity result = FindEntity(entity.Name);
            Debug.Assert(result == null, "Entity '" + entity.Name + "' already exists");
            entityList.Add(entity);
        }

        public Entity ReturnEntity(string Name) 
        {
            return FindEntity(Name);
        }

        public void RemoveEntity(Entity entity)
        {
            entityList.Remove(entity);
        }
        private Entity FindEntity(string name)
        {
            return entityList.Find(delegate(Entity e)
            {
                return e.Name == name;
            }
            );
        }
        public void Close()
        {
            
        }
        public List<Entity> Entities()
        {
            return entityList;
        }
    }
}
