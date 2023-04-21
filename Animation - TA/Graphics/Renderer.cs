using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;

//include GLM library
using GlmNet;
using System.IO;
using System.Diagnostics;

namespace Graphics
{
    class Renderer
    {
        Shader sh;
        
        uint triangleBufferID;
        
        //3D Drawing
        mat4 ModelMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
        
        int ShaderModelMatrixID;
        int ShaderViewMatrixID;
        int ShaderProjectionMatrixID;

        const float rotationSpeed = 3f;
        float rotationAngle = 0;

        public float translationX=0, 
                     translationY=0, 
                     translationZ=0;

        Stopwatch timer = Stopwatch.StartNew();

        vec3 tableCenter;
        Texture tex1;

        public void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            tex1 = new Texture(projectPath + "\\Textures\\crate.jpg", 1);
            Gl.glClearColor(0, 0, 0.4f, 1);
            
            float[] table= { 
		         //el table
                2.0f, 4.0f, 2.0f,
                0.0f, 0.0f, 0.0f,0,0,
                2.0f, 4.0f, 8.0f,
                0.0f, 0.0f, 0.0f,0,1,
                8.0f, 4.0f, 2.0f,
                0.0f, 0.0f, 0.0f,1,0,
                8.0f, 4.0f, 8.0f,
                0.0f, 0.0f, 0.0f,1,1,
                // 4 regol el table
                2.0f, 4.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.0f, 2.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.1f, 2.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.1f, 4.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,

                2.0f, 4.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.0f, 2.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.1f, 2.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                2.1f, 4.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,

                8.0f, 4.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                8.0f, 2.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                7.9f, 2.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,
                7.9f, 4.0f, 2.0f,
                1.0f, 0.0f, 0.0f,0,0,

                8.0f, 4.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                8.0f, 2.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                7.9f, 2.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,
                7.9f, 4.0f, 8.0f,
                1.0f, 0.0f, 0.0f,0,0,

                // el polygon
                4.0f, 4.0f, 6.0f,
                0.0f, 1.0f, 1.0f,0,0,
                6.0f, 4.0f, 6.0f,
                0.0f, 1.0f, 1.0f,0,0,
                6.5f, 4.0f, 5.0f,
                0.0f, 1.0f, 1.0f,0,0,
                6.0f, 4.0f, 4.0f,
                0.0f, 1.0f, 1.0f,0,0,
                4.0f, 4.0f, 4.0f,
                0.0f, 1.0f, 1.0f,0,0,
                3.5f, 4.0f, 5.0f,
                0.0f, 1.0f, 1.0f,0,0,
                // 3 triangle 
                4.0f, 6.0f, 6.0f,
                1.0f, 0.0f, 0.0f,0,0,
                3.5f, 4.0f, 5.0f,
                0.0f, 1.0f, 0.0f,0,0,
                6.0f, 4.0f, 4.0f,
                0.0f, 0.0f, 1.0f,0,0,

                6.5f, 4.0f, 5.0f,
                1.0f, 1.0f, 0.0f,0,0,
                4.0f, 4.0f, 4.0f,
                1.0f, 1.0f, 0.0f,0,0,
                6.0f, 6.0f, 4.0f,
                1.0f, 1.0f, 0.0f,0,0,

                6.0f, 4.0f, 6.0f,
                0.5f, 1.0f, 0.5f,0,0,
                4.0f, 4.0f, 4.0f,
                1.0f, 1.0f, 0.0f,0,0,
                3.5f, 6.0f, 5.0f,
                0.0f, 1.0f, 0.0f,0,0,

            }; 
            
            tableCenter = new vec3(5.0f, 4.0f, 5.0f);

            


            triangleBufferID = GPU.GenerateBuffer(table);
            

            // View matrix 
            ViewMatrix = glm.lookAt(
                new vec3(0, 6, 20),// eye
                new vec3(3, 5, 5), // center
                new vec3(0, 20, 0) // up
                );
            // Model Matrix Initialization
            ModelMatrix = new mat4(1);

            //ProjectionMatrix = glm.perspective(FOV, Width / Height, Near, Far);
            ProjectionMatrix = glm.perspective(45.0f, 4.0f / 3.0f, 0.1f, 100.0f);
            
            // Our MVP matrix which is a multiplication of our 3 matrices 
            sh.UseShader();


            //Get a handle for our "MVP" uniform (the holder we created in the vertex shader)
            ShaderModelMatrixID = Gl.glGetUniformLocation(sh.ID, "modelMatrix");
            ShaderViewMatrixID = Gl.glGetUniformLocation(sh.ID, "viewMatrix");
            ShaderProjectionMatrixID = Gl.glGetUniformLocation(sh.ID, "projectionMatrix");

            Gl.glUniformMatrix4fv(ShaderViewMatrixID, 1, Gl.GL_FALSE, ViewMatrix.to_array());
            Gl.glUniformMatrix4fv(ShaderProjectionMatrixID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());

            timer.Start();
        }

        public void Draw()
        {
            sh.UseShader();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);

            #region Animated Triangle
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, triangleBufferID);

            Gl.glUniformMatrix4fv(ShaderModelMatrixID, 1, Gl.GL_FALSE, ModelMatrix.to_array());

            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            tex1.Bind();
            Gl.glDrawArrays(Gl.GL_QUAD_STRIP, 0, 4);
            Gl.glDrawArrays(Gl.GL_LINE_LOOP, 4, 4);
            Gl.glDrawArrays(Gl.GL_LINE_LOOP, 8, 4);
            Gl.glDrawArrays(Gl.GL_LINE_LOOP, 12, 4);
            Gl.glDrawArrays(Gl.GL_LINE_LOOP, 16, 4);


            Gl.glDrawArrays(Gl.GL_POLYGON, 20, 6);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 26, 3);
            Gl.glDrawArrays(Gl.GL_TRIANGLE_STRIP, 29, 3);
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 32, 3);



            Gl.glDisableVertexAttribArray(0);
            Gl.glDisableVertexAttribArray(1);
            Gl.glDisableVertexAttribArray(2);
            #endregion
        }
        
        public void Update()
        {
            timer.Stop();
            var deltaTime = timer.ElapsedMilliseconds/1000.0f;
            rotationAngle += deltaTime * rotationSpeed;

            List<mat4> transformations = new List<mat4>();
            transformations.Add(glm.translate(new mat4(1), -1 * tableCenter));
            transformations.Add(glm.rotate(rotationAngle, new vec3(0, 1, 0)));
            transformations.Add(glm.translate(new mat4(1), tableCenter));
            transformations.Add(glm.translate(new mat4(1), new vec3(translationX, translationY, translationZ)));

            ModelMatrix =  MathHelper.MultiplyMatrices(transformations);
            
            timer.Reset();
            timer.Start();
        }
        
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
