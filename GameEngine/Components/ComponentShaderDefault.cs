using OpenGL_Game.Scenes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenGL_Game.OBJLoader;

namespace OpenGL_Game.Components
{
    class ComponentShaderDefault : ComponentShader
    {
        //the default shader used
        //creates the uniforms needed in the fragment and vertex shaders
        public int uniform_stex;
        public int uniform_mmodelviewproj;
        public int uniform_mmodel;
        public int uniform_diffuse;
        //creates a list of uniform lights positions and colours
        public int[] uniform_LightPosition = new int[4];
        public int[] uniform_LightColour = new int[4];
        
        //creates a list of positions and colors
        public Vector3[] LightPos = new Vector3[4];
        public Vector3[] LightCol = new Vector3[4];


        public ComponentShaderDefault() : base("Shaders/vs.glsl", "Shaders/fs.glsl")
        {
            //sets the uniforms locations
            uniform_stex = GL.GetUniformLocation(pgmID, "s_texture");
            uniform_mmodelviewproj = GL.GetUniformLocation(pgmID, "ModelViewProjMat");
            uniform_mmodel = GL.GetUniformLocation(pgmID, "ModelMat");
            uniform_diffuse = GL.GetUniformLocation(pgmID, "v_diffuse");

            for (int i = 0; i < 4; i++)
            {
                uniform_LightPosition[i] = GL.GetUniformLocation(pgmID, "LightPosition[" + i + "]");
                uniform_LightColour[i] = GL.GetUniformLocation(pgmID, "LightColour[" + i + "]");
            }
            //sets the lights positions and colours
            LightPos[0] = new Vector3(0,40,0);
            LightPos[1] = new Vector3(80,40,0);
            LightPos[2] = new Vector3(0,40,80);
            LightPos[3] = new Vector3(80,40,80);

            LightCol[0] = new Vector3(0.5f, 0.2f, 0.5f);
            LightCol[1] = new Vector3(1f, 0.0f, 0.0f);
            LightCol[2] = new Vector3(0.0f, 1f, 0.0f);
            LightCol[3] = new Vector3(0.0f, 0.0f, 1f);
        }

        public override void ApplyShader(Matrix4 model, Geometry geometry)
        {

            GL.UseProgram(pgmID);
            //sends the light data to the shaders probably
            for (int i = 0; i < 4; i++)
            {   
                GL.Uniform3(uniform_LightPosition[i], LightPos[i]);
                GL.Uniform3(uniform_LightColour[i], LightCol[i]);
            }

            GL.Uniform1(uniform_stex, 0);
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.UniformMatrix4(uniform_mmodel, false, ref model);
            Matrix4 modelViewProjection = model * GameScene.gameInstance.camera.view * GameScene.gameInstance.camera.projection;
            GL.UniformMatrix4(uniform_mmodelviewproj, false, ref modelViewProjection);

            geometry.Render(uniform_diffuse);

            GL.UseProgram(0);
        }

    }
}