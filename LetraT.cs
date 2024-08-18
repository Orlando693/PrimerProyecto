using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
//hola luis

namespace PrimerProyecto
{
    public class LetraT : GameWindow
    {
        private int VerticeBufferHandle;
        private int ElementBufferHandle;
        private int ShaderProgramHandle;
        private int VertexArrayHandle;
        private Matrix4 projectionMatrix;
        private Matrix4 viewMatrix;
        private Matrix4 modelMatrix;

      

            // Definimos los vértices de la letra "T" en 3D
            float[] vertices = new float[]
            {
                // Frente de la T
                -0.1f, -0.5f,  0.1f, // 0
                 0.1f, -0.5f,  0.1f, // 1
                 0.1f,  0.3f,  0.1f, // 2
                -0.1f,  0.3f,  0.1f, // 3

                -0.5f,  0.3f,  0.1f, // 4
                 0.5f,  0.3f,  0.1f, // 5
                 0.5f,  0.5f,  0.1f, // 6
                -0.5f,  0.5f,  0.1f, // 7

                // Atrás de la T
                -0.1f, -0.5f, -0.1f, // 8
                 0.1f, -0.5f, -0.1f, // 9
                 0.1f,  0.3f, -0.1f, // 10
                -0.1f,  0.3f, -0.1f, // 11

                -0.5f,  0.3f, -0.1f, // 12
                 0.5f,  0.3f, -0.1f, // 13
                 0.5f,  0.5f, -0.1f, // 14
                -0.5f,  0.5f, -0.1f  // 15
            };

        // Definimos los índices para formar las líneas
        uint[] indices = new uint[]
        {
                // Líneas de la parte frontal de la "T"
                0, 1, 1, 2, 3, 0, 4, 3, 2, 5,  // Cuadrado central
                5, 6, 6, 7, 7, 4,  // Cuadrado superior

                // Líneas de la parte trasera de la "T"
                8, 9, 9, 10, 11, 8,12,11,10,13,  // Cuadrado central
                13, 14, 14, 15, 15, 12, // Cuadrado superior

                // Líneas de conexión entre las caras frontal y trasera
                0, 8, 1, 9,  // Conectores del cuadrado central
                4, 12, 5, 13, 6, 14, 7, 15 // Conectores del cuadrado superior   
        }; 
        public LetraT()
            : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(1100, 668));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)e.Width / e.Height, 0.1f, 100f);
            base.OnResize(e);
        }
         
         

        protected override void OnLoad() //Cargar
        {
            GL.ClearColor(new Color4(0.2f, 0.9f, 0.8f, 1f));
          

            // Crear el VBO (Vertex Buffer Object)
            this.VerticeBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VerticeBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Crear el EBO (Element Buffer Object)
            this.ElementBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Crear el VAO (Vertex Array Object)
            this.VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.VertexArrayHandle);

            // Configurar el atributo de posición
            GL.BindBuffer(BufferTarget.ArrayBuffer, this.VerticeBufferHandle);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.ElementBufferHandle);

            GL.BindVertexArray(0);

            // Compilar el Vertex Shader
            string vertexShaderCode =
                @"
                #version 330 core
                layout (location = 0) in vec3 aPosition;
                
                uniform mat4 projection;
                uniform mat4 view;
                uniform mat4 model;

                void main(){
                    gl_Position = projection * view * model * vec4(aPosition, 1f);
                }
                ";
            int VerticeShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VerticeShaderHandle, vertexShaderCode);
            GL.CompileShader(VerticeShaderHandle);

            // Compilar el Fragment Shader
            string pixelShaderCode =
                @"
                #version 330 core
                
                out vec4 pixelColor;
                
                void main(){
                    pixelColor = vec4(0f, 0f, 0f, 1f);   // Color negro para las líneas
                }
                ";
            int pixelShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderHandle, pixelShaderCode);
            GL.CompileShader(pixelShaderHandle);

            // Vincular los shaders al programa
            this.ShaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(this.ShaderProgramHandle, VerticeShaderHandle);
            GL.AttachShader(this.ShaderProgramHandle, pixelShaderHandle);
            GL.LinkProgram(this.ShaderProgramHandle);

            // Desvincular y eliminar shaders
            GL.DetachShader(this.ShaderProgramHandle, VerticeShaderHandle);
            GL.DetachShader(this.ShaderProgramHandle, pixelShaderHandle);
            GL.DeleteShader(VerticeShaderHandle);
            GL.DeleteShader(pixelShaderHandle);

            // Configuración de las matrices
            modelMatrix = Matrix4.Identity;
            viewMatrix = Matrix4.LookAt(new Vector3(1.5f, 1.5f, 3f), Vector3.Zero, Vector3.UnitY);
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Size.X / (float)Size.Y, 0.1f, 100f);

            base.OnLoad();
        }

        protected override void OnUnload() //liberar memoria
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(this.VerticeBufferHandle);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(this.ElementBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.ShaderProgramHandle);

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.ShaderProgramHandle);
            GL.BindVertexArray(this.VertexArrayHandle);

            int modelLoc = GL.GetUniformLocation(this.ShaderProgramHandle, "model");
            GL.UniformMatrix4(modelLoc, false, ref modelMatrix);

            int viewLoc = GL.GetUniformLocation(this.ShaderProgramHandle, "view");
            GL.UniformMatrix4(viewLoc, false, ref viewMatrix);

            int projectionLoc = GL.GetUniformLocation(this.ShaderProgramHandle, "projection");
            GL.UniformMatrix4(projectionLoc, false, ref projectionMatrix);

            // Dibujar las líneas de la letra "T"
            GL.DrawElements(PrimitiveType.Lines, 48, DrawElementsType.UnsignedInt, 0);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}



