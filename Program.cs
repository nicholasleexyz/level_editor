using System.Numerics;
using Raylib_cs;

class Program
{
    public static void Main()
    {
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 1024;
        const int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
        const int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;
        const int CELL_SIZE = 64;


        Vector2 mousePos;
        Vector2 target = Vector2.Zero;
        Camera2D cam = new Camera2D(Vector2.Zero, target, 0, 1f);

        float camMoveSpeed = 400f;

        const int rows = 20;
        const int columns = 40;
        const int totalCellCount = rows * columns;

        var xOffset = HALF_SCREEN_WIDTH - (CELL_SIZE * columns * 0.5f);
        var yOffset = HALF_SCREEN_HEIGHT - (CELL_SIZE * rows * 0.5f);

        Rectangle[] cells = new Rectangle[totalCellCount];

        for (int j = 0; j < rows; j++)
        {
            for (int i = 0; i < columns; i++)
            {
                float gap = 0;
                float x = CELL_SIZE * i + gap * i + xOffset;
                float y = CELL_SIZE * j + gap * j + yOffset;
                float width = CELL_SIZE;
                float height = CELL_SIZE;

                cells[j * columns + i] = new Rectangle(x, y, width, height);
            }
        }

        // for (int i = 0; i < totalCellCount; i++)
        // {
        //     float gap = 0;
        //     float x = CELL_SIZE * (i % columns) + gap * (i % columns) + xOffset;
        //     float y = CELL_SIZE * (i / rows) + gap * (i / rows) + yOffset;
        //     float width = CELL_SIZE;
        //     float height = CELL_SIZE;

        //     cells[i / rows + i%columns] = new Rectangle(x, y, width, height);
        // }

        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Level Editor");

        while (!Raylib.WindowShouldClose())
        {
            mousePos = Raylib.GetMousePosition();

            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            {
                target += new Vector2(camMoveSpeed * Raylib.GetFrameTime(), 0);
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            {
                target -= new Vector2(camMoveSpeed * Raylib.GetFrameTime(), 0);
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            {
                target -= new Vector2(0, camMoveSpeed * Raylib.GetFrameTime());
            }
            if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            {
                target += new Vector2(0, camMoveSpeed * Raylib.GetFrameTime());
            }

            cam.target = target;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.GRAY);

            Raylib.BeginMode2D(cam);

            foreach (var r in cells)
            {
                Raylib.DrawRectangleLinesEx(r, 2, Color.DARKBLUE);

                if (Raylib.CheckCollisionPointRec(mousePos + target, r))
                {
                    Raylib.DrawRectangle((int)r.x, (int)r.y, (int)r.width, (int)r.height, Color.PURPLE);
                    Raylib.DrawRectangleLinesEx(r, 2, Color.WHITE);
                }
            }

            Raylib.DrawFPS(10 + (int)(target.X), 10 + (int)target.Y);
            Raylib.EndMode2D();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}