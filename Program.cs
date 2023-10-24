using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

class Program
{
    public static void Main()
    {
        const string pathBackgroundTexture = "./assets/back.png";
        const string pathCrateTexture = "./assets/crate.png";

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

        Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "Level Editor");
        Texture2D background = Raylib.LoadTexture(pathBackgroundTexture);
        Texture2D crate = Raylib.LoadTexture(pathCrateTexture);

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

            // draw background
            Raylib.DrawTexturePro(
                background,
                new Rectangle(0, 0, background.width, background.height),
                new Rectangle(target.X, target.Y, SCREEN_WIDTH, SCREEN_HEIGHT),
                Vector2.Zero,
                0,
                Color.WHITE);

            foreach (var r in cells)
            {
                Raylib.DrawRectangleLinesEx(r, 2, Color.DARKBLUE);

                if (Raylib.CheckCollisionPointRec(mousePos + target, r))
                {
                    // Draw crate
                    Raylib.DrawTexturePro(
                        crate,
                        new Rectangle(0, 0, crate.width, crate.height),
                        r,
                        new Vector2(0, 0),
                        0,
                        Color.WHITE);

                    // crate outline
                    Raylib.DrawRectangleLinesEx(r, 1, Color.WHITE);
                }
            }


            void btn(
                int x,
                int y,
                int width,
                int height,
                Color bgColor,
                Color exColor,
                Color bgColorHover,
                Color exColorHover,
                Action onClick)
            {
                var btnRec = new Rectangle(x, y, width, height);
                var btnCol = bgColor;
                var btnExCol = exColor;

                if (Raylib.CheckCollisionPointRec(mousePos + target, btnRec))
                {
                    btnCol = bgColorHover;
                    btnExCol = exColorHover;
                    if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                    {
                        onClick();
                    }
                }

                Raylib.DrawRectangle(x, y, width, height, btnCol);
                Raylib.DrawRectangleLinesEx(btnRec, 2, btnExCol);

                //magic numbers used here :p
                var textX = (width + x) / 2 - (4 * 16);
                var textY = (height + y) / 2 - 24;

                Raylib.DrawText("asdf", textX, textY, 64, Color.BLACK);
            }

            var _onClick = () => Console.WriteLine("test");

            btn(10, 10, 300, 100, Color.BLUE, Color.DARKBLUE, Color.GREEN, Color.WHITE, _onClick);


            Raylib.DrawFPS(10 + (int)(target.X), 10 + (int)target.Y);
            Raylib.EndMode2D();

            Raylib.EndDrawing();
        }

        Raylib.UnloadTexture(background);
        Raylib.UnloadTexture(crate);
        Raylib.CloseWindow();
    }
}