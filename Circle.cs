using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Point = Microsoft.Xna.Framework.Point;
using static SDL2.SDL;

namespace RasterFna {
    internal static class Circle {
        public static void DrawCircle(IntPtr renderer, int x, int y, int radius, bool fill = false) {
            radius = Math.Abs(radius);

            if (radius == 0) {
                SDL_RenderDrawPoint(renderer, x, y);
                return;
            }

            var diameter = radius * 2 + 1;
            if (diameter == 0) {
                return;
            }

            var xOffset = 0;
            var yOffset = radius;

            var p = 1 - radius;

            if (fill) {
                // top/bottom
                while (xOffset <= yOffset) {
                    if (p < 0) {
                        p = p + 2 * xOffset + 3;
                    }
                    else {
                        var x0 = x - xOffset;
                        var x1 = x + xOffset;
                        var y0 = y - yOffset;
                        var y1 = y + yOffset;

                        if (x0 == x1) {
                            // workaround for SDL issue with a single pixel line not drawing at all
                            SDL_RenderDrawPoint(renderer, x0, y0);
                            SDL_RenderDrawPoint(renderer, x0, y1);
                        }
                        else {
                            SDL_RenderDrawLine(renderer, x0, y0, x1, y0);
                            SDL_RenderDrawLine(renderer, x0, y1, x1, y1);
                        }

                        p = p + 2 * (xOffset - yOffset) + 5;
                        yOffset--;
                    }

                    xOffset++;
                }

                xOffset = 0;
                yOffset = radius;

                p = 1 - radius;

                // center
                while (xOffset <= yOffset) {
                    if (xOffset != yOffset) {
                        var x0 = x - yOffset;
                        var x1 = x + yOffset;
                        var y0 = y - xOffset;

                        SDL_RenderDrawLine(renderer, x0, y0, x1, y0);

                        if (xOffset != 0) {
                            var y1 = y + xOffset;
                            SDL_RenderDrawLine(renderer, x0, y1, x1, y1);
                        }
                    }

                    if (p < 0) {
                        p = p + 2 * xOffset + 3;
                    }
                    else {
                        p = p + 2 * (xOffset - yOffset) + 5;
                        yOffset--;
                    }

                    xOffset++;
                }
            }
            else {
                while (xOffset <= yOffset) {
                    var x0 = x - xOffset;
                    var x2 = x - yOffset;
                    var x3 = x + yOffset;
                    var y0 = y - yOffset;
                    var y1 = y + yOffset;
                    var y2 = y - xOffset;

                    // top/bottom
                    SDL_RenderDrawPoint(renderer, x0, y0);
                    SDL_RenderDrawPoint(renderer, x0, y1);

                    if (xOffset != 0) {
                        var x1 = x + xOffset;
                        SDL_RenderDrawPoint(renderer, x1, y0);
                        SDL_RenderDrawPoint(renderer, x1, y1);
                    }

                    // center
                    if (xOffset != yOffset) {
                        SDL_RenderDrawPoint(renderer, x2, y2);
                        SDL_RenderDrawPoint(renderer, x3, y2);

                        if (xOffset != 0) {
                            var y3 = y + xOffset;
                            SDL_RenderDrawPoint(renderer, x2, y3);
                            SDL_RenderDrawPoint(renderer, x3, y3);
                        }
                    }

                    if (p < 0) {
                        p = p + 2 * xOffset + 3;
                    }
                    else {
                        p = p + 2 * (xOffset - yOffset) + 5;
                        yOffset--;
                    }

                    xOffset++;
                }
            }
        }
    }
}
