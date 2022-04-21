using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using static SDL2.SDL;

namespace RasterFna {
    public class Drawable : IDisposable {
        private int width;
        private int height;

        private IntPtr surfPtr;
        private IntPtr renderer;
        private Color renderColor;

        private GraphicsDevice graphics;
        private Texture2D texture;

        public Drawable(GraphicsDevice graphics, int width, int height) {
            this.graphics = graphics;
            this.width = width;
            this.height = height;

            surfPtr = SDL_CreateRGBSurfaceWithFormat(0, this.width, this.height, 32, SDL_PIXELFORMAT_ABGR8888);
            if (surfPtr == IntPtr.Zero) {
                throw new Exception($"Couldn't create SDL2 surface: {SDL_GetError()}");
            }

            SurfaceBlend(SDL_BlendMode.SDL_BLENDMODE_BLEND);

            renderer = SDL_CreateSoftwareRenderer(surfPtr);
            if (renderer == IntPtr.Zero) {
                throw new Exception($"Couldn't create SDL2 software renderer for surface: {SDL_GetError()}");
            }

            RenderBlend(SDL_BlendMode.SDL_BLENDMODE_BLEND);

            renderColor = Color.White;
            SDL_SetRenderDrawColor(renderer, renderColor.R, renderColor.G, renderColor.B, renderColor.A);
        }

        private void RenderBlend(SDL_BlendMode blend) {
            if (SDL_SetRenderDrawBlendMode(renderer, blend) != 0) {
                throw new Exception($"Couldn't set blend mode for SDL2 renderer: {SDL_GetError()}");
            }
        }

        private void SurfaceBlend(SDL_BlendMode blend) {
            if (SDL_SetSurfaceBlendMode(surfPtr, blend) != 0) {
                throw new Exception($"Couldn't set blend mode for SDL2 surface: {SDL_GetError()}");
            }
        }

        public void Clear(Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            if (SDL_RenderClear(renderer) != 0) {
                throw new Exception($"Couldn't clear SDL2 surface: {SDL_GetError()}");
            }
        }

        public void Clear(Rectangle rect, Color color) {
            Clear(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public void Clear(int x, int y, int width, int height, Color color) {
            RenderBlend(SDL_BlendMode.SDL_BLENDMODE_NONE);
            FillRect(x, y, width, height, color);
            RenderBlend(SDL_BlendMode.SDL_BLENDMODE_BLEND);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPixel(int x, int y, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            SDL_RenderDrawPoint(renderer, x, y);
        }

        public void DrawLine(float x1, float y1, float x2, float y2, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            if (SDL_RenderDrawLineF(renderer, x1, y1, x2, y2) != 0) {
                throw new Exception($"Couldn't draw line on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            if (SDL_RenderDrawLine(renderer, x1, y1, x2, y2) != 0) {
                throw new Exception($"Couldn't draw line on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawRect(Rectangle rect, Color color) {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public void DrawRect(int x, int y, int width, int height, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }

            SDL_Rect r = new SDL_Rect() { x = x, y = y, w = width, h = height };
            if (SDL_RenderDrawRect(renderer, ref r) != 0) {
                throw new Exception($"Couldn't draw rect on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void FillRect(Rectangle rect, Color color) {
            FillRect(rect.X, rect.Y, rect.Width, rect.Height, color);
        }

        public void FillRect(int x, int y, int width, int height, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }

            SDL_Rect r = new SDL_Rect() { x = x, y = y, w = width, h = height };
            if (SDL_RenderFillRect(renderer, ref r) != 0) {
                throw new Exception($"Couldn't fill rect on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawCircle(int x, int y, int radius, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            Circle.DrawCircle(renderer, x, y, radius, false);
        }

        public void FillCircle(int x, int y, int radius, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            Circle.DrawCircle(renderer, x, y, radius, true);
        }

        public void DrawEllipse(int x, int y, int radX, int radY, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            Ellipse.DrawEllipse(renderer, x, y, radX, radY, false);
        }

        public void FillEllipse(int x, int y, int radX, int radY, Color color) {
            if (renderColor != color) {
                SDL_SetRenderDrawColor(renderer, color.R, color.G, color.B, color.A);
                renderColor = color;
            }
            Ellipse.DrawEllipse(renderer, x, y, radX, radY, true);
        }

        public void Draw(Drawable image, int x, int y) {
            SDL_Rect dst = new SDL_Rect() { x = x, y = y };
            if (SDL_BlitSurface(image.surfPtr, IntPtr.Zero, surfPtr, ref dst) != 0) {
                throw new Exception($"Couldn't draw image on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawSrc(Drawable image, int x, int y, Rectangle sourceRect) {
            DrawSrc(image, x, y, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
        }

        public void DrawSrc(Drawable image, int x, int y, int srcX, int srcY, int srcWidth, int srcHeight) {
            SDL_Rect src = new SDL_Rect() { x = srcX, y = srcY, w = srcWidth, h = srcHeight };
            SDL_Rect dst = new SDL_Rect() { x = x, y = y };
            if (SDL_BlitSurface(image.surfPtr, ref src, surfPtr, ref dst) != 0) {
                throw new Exception($"Couldn't draw image on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawScaled(Drawable image, Rectangle destRect) {
            DrawScaled(image, destRect.X, destRect.Y, destRect.Width, destRect.Height);
        }

        public void DrawScaled(Drawable image, int x, int y, int width, int height) {
            SDL_Rect dst = new SDL_Rect() { x = x, y = y, w = width, h = height };
            if (SDL_BlitScaled(image.surfPtr, IntPtr.Zero, surfPtr, ref dst) != 0) {
                throw new Exception($"Couldn't draw image on SDL2 surface: {SDL_GetError()}");
            }
        }

        public void DrawSrcScaled(Drawable image, Rectangle destRect, Rectangle sourceRect) {
            DrawSrcScaled(image, destRect.X, destRect.Y, destRect.Width, destRect.Height, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
        }

        public void DrawSrcScaled(Drawable image, int dstX, int dstY, int dstWidth, int dstHeight, Rectangle sourceRect) {
            DrawSrcScaled(image, dstX, dstY, dstWidth, dstHeight, sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
        }

        public void DrawSrcScaled(Drawable image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight) {
            DrawSrcScaled(image, destRect.X, destRect.Y, destRect.Width, destRect.Height, srcX, srcY, srcWidth, srcHeight);
        }

        public void DrawSrcScaled(Drawable image, int dstX, int dstY, int dstWidth, int dstHeight, int srcX, int srcY, int srcWidth, int srcHeight) {
            SDL_Rect src = new SDL_Rect() { x = srcX, y = srcY, w = srcWidth, h = srcHeight };
            SDL_Rect dst = new SDL_Rect() { x = dstX, y = dstY, w = dstWidth, h = dstHeight };
            if (SDL_BlitScaled(image.surfPtr, ref src, surfPtr, ref dst) != 0) {
                throw new Exception($"Couldn't draw image on SDL2 surface: {SDL_GetError()}");
            }
        }

        public Texture2D MakeTexture() {
            if (texture == null || texture.IsDisposed) {
                texture = new Texture2D(graphics, width, height);
            }

            if (SDL_LockSurface(surfPtr) != 0) {
                throw new Exception($"Couldn't lock SDL2 surface in MakeTexture: {SDL_GetError()}");
            }

            SDL_Surface desc = surfaceDesc;
            texture.SetDataPointerEXT(0, null, desc.pixels, desc.pitch * desc.h);

            SDL_UnlockSurface(surfPtr);

            return texture;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    if (texture != null && !texture.IsDisposed) {
                        texture.Dispose();
                    }
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.
                SDL_FreeSurface(surfPtr);
                SDL_DestroyRenderer(renderer);

                graphics = null;
                texture = null;

                disposedValue = true;
            }
        }

        ~Drawable() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private SDL_Surface surfaceDesc {
            get {
                return surfPtr != IntPtr.Zero ? (SDL_Surface)Marshal.PtrToStructure(
                    surfPtr,
                    typeof(SDL_Surface)
                ) : new SDL_Surface();
            }
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
    }
}
