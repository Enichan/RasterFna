using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using static SDL2.SDL;

namespace RasterFna {
    internal static class Ellipse {
        public static void DrawEllipse(IntPtr renderer, int x, int y, int radx, int rady, bool fill = false) {
            if (radx == rady) {
                // hey, this is a circle! >:(
                Circle.DrawCircle(renderer, x, y, radx, fill);
                return;
            }

            radx = Math.Abs(radx);
            rady = Math.Abs(rady);

            var xDiameter = radx * 2 + 1;
            var yDiameter = rady * 2 + 1;

            if (xDiameter == 0 || yDiameter == 0) {
                return;
            }

            long xx2 = 2 * radx * radx;
            long yy2 = 2 * rady * rady;

            long xOffset = radx;
            long yOffset = 0;

            long dx = rady * rady * (1 - 2 * radx);
            long dy = radx * radx;
            long p = 0;

            long sx = yy2 * radx;
            long sy = 0;

            if (fill) {
                // center
                while (sx >= sy) {
                    var x0 = (int)(x - xOffset);
                    var x1 = (int)(x + xOffset);
                    var y0 = (int)(y - yOffset);
                    var y1 = (int)(y + yOffset);

                    if (x0 == x1) {
                        // workaround for SDL issue with a single pixel line not drawing at all
                        SDL_RenderDrawPoint(renderer, x0, y0);
                        SDL_RenderDrawPoint(renderer, x0, y1);
                    }
                    else {
                        SDL_RenderDrawLine(renderer, x0, y0, x1, y0);
                        if (yOffset != 0) {
                            SDL_RenderDrawLine(renderer, x0, y1, x1, y1);
                        }
                    }

                    yOffset++;
                    sy += xx2;
                    p += dy;
                    dy += xx2;

                    if (2 * p + dx > 0) {
                        xOffset--;
                        sx -= yy2;
                        p += dx;
                        dx += yy2;
                    }
                }

                xOffset = 0;
                yOffset = rady;

                dx = rady * rady;
                dy = radx * radx * (1 - 2 * rady);
                p = 0;

                sx = 0;
                sy = xx2 * rady;

                // top/bottom
                while (sx <= sy) {
                    xOffset++;
                    sx += yy2;
                    p += dx;
                    dx += yy2;

                    if (2 * p + dy > 0) {
                        var x0 = (int)(x - xOffset);
                        var x1 = (int)(x + xOffset);
                        var y0 = (int)(y - yOffset);
                        var y1 = (int)(y + yOffset);

                        if (x0 == x1) {
                            // workaround for SDL issue with a single pixel line not drawing at all
                            SDL_RenderDrawPoint(renderer, x0, y0);
                            SDL_RenderDrawPoint(renderer, x0, y1);
                        }
                        else {
                            SDL_RenderDrawLine(renderer, x0, y0, x1, y0);
                            SDL_RenderDrawLine(renderer, x0, y1, x1, y1);
                        }

                        yOffset--;
                        sy -= xx2;
                        p += dy;
                        dy += xx2;
                    }
                }
            }
            else {
                // center
                while (sx >= sy) {
                    var x0 = (int)(x - xOffset);
                    var x1 = (int)(x + xOffset);
                    var y0 = (int)(y - yOffset);

                    SDL_RenderDrawPoint(renderer, x0, y0);
                    SDL_RenderDrawPoint(renderer, x1, y0);

                    if (yOffset != 0) {
                        var y1 = (int)(y + yOffset);
                        SDL_RenderDrawPoint(renderer, x0, y1);
                        SDL_RenderDrawPoint(renderer, x1, y1);
                    }

                    yOffset++;
                    sy += xx2;
                    p += dy;
                    dy += xx2;

                    if (2 * p + dx > 0) {
                        xOffset--;
                        sx -= yy2;
                        p += dx;
                        dx += yy2;
                    }
                }

                xOffset = 0;
                yOffset = rady;

                dx = rady * rady;
                dy = radx * radx * (1 - 2 * rady);
                p = 0;

                sx = 0;
                sy = xx2 * rady;

                // top/bottom
                while (sx <= sy) {
                    var x0 = (int)(x - xOffset);
                    var y0 = (int)(y - yOffset);
                    var y1 = (int)(y + yOffset);

                    SDL_RenderDrawPoint(renderer, x0, y0);
                    SDL_RenderDrawPoint(renderer, x0, y1);

                    if (xOffset != 0) {
                        var x1 = (int)(x + xOffset);
                        SDL_RenderDrawPoint(renderer, x1, y0);
                        SDL_RenderDrawPoint(renderer, x1, y1);
                    }

                    xOffset++;
                    sx += yy2;
                    p += dx;
                    dx += yy2;

                    if (2 * p + dy > 0) {
                        yOffset--;
                        sy -= xx2;
                        p += dy;
                        dy += xx2;
                    }
                }
            }
        }
    }
}
