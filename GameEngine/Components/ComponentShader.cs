using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenGL_Game.Components
{
    abstract class ComponentShader : IComponent
    {
        //sets up the shader program for other shaders to inherit from
        public int pgmID;

        public ComponentShader(string vertexName, string fragementName)
        {
            pgmID = GL.CreateProgram();
            GL.AttachShader(pgmID, ResourceManager.LoadShader(vertexName, ShaderType.VertexShader));
            GL.AttachShader(pgmID, ResourceManager.LoadShader(fragementName, ShaderType.FragmentShader));
            GL.LinkProgram(pgmID);
        }

        public abstract void ApplyShader(Matrix4 model, Geometry geometry);

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_SHADER; }
        }
    }
}
