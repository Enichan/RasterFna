# RasterFna

SDL2 based software rasterizer for use with the FNA library. Very early iteration, not stable. Probably don't use this for production, but idk I'm not your boss.

To compile this library clone or download FNA next to this repository so that both RasterFna and FNA folders exist side by side.  

## Usage

```csharp
using (var drawable = new Drawable(GraphicsDevice, 320, 200)) {
    drawable.Clear(Color.CornflowerBlue);
    drawable.DrawLine(0, 0, 40, 20, Color.Black);
    drawable.FillRect(50, 5, 80, 30, Color.White);
    drawable.DrawCircle(160, 100, 10, Color.Red);
    drawable.FillEllipse(280, 150, 30, 15, Color.Blue);
    
    var random = new Random(1234);
    for (int i = 0; i < 100; i++) {
        drawable.SetPixel(random.Next(drawable.Width), random.Next(drawable.Height), Color.Green);
    }
    
    using (var batch = new SpriteBatch(GraphicsDevice)) {
        batch.Begin();
        // don't call Dispose on the Texture2D returned by MakeTexture
        batch.Draw(drawable.MakeTexture(), Vector2.Zero, Color.White);
        batch.End();
    }
}
```

You can also draw `Drawable` instances to each other using `Draw`, `DrawSrc`, `DrawScaled`, and `DrawSrcScaled`.