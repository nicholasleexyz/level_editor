using System.Numerics;
using Raylib_cs;


class Program
{
    enum TextureType
    {
        Background,
        Block,
        Crate,
        Face,
        Rock
    }

    public static void Main()
    {
        const int SCREEN_WIDTH = 1024;
        const int SCREEN_HEIGHT = 1024;
        const int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
        const int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;
        const int CELL_SIZE = 64;

        Dictionary<TextureType, string> texurePaths = new() {
            {TextureType.Background, "./assets/back.png"},
            {TextureType.Block, "./assets/block.png"},
            {TextureType.Crate, "./assets/crate.png"},
            {TextureType.Face, "./assets/face.png"},
            {TextureType.Rock, "./assets/rock.png"},
        };

        Vector2 mousePos;
        Vector2 target = Vector2.Zero;
        Camera2D cam = new Camera2D(Vector2.Zero, target, 0, 1f);

        float camMoveSpeed = 400f;

        const int rows = 10;
        const int columns = 12;
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
        Texture2D background = Raylib.LoadTexture(texurePaths[TextureType.Background]);
        Texture2D crate = Raylib.LoadTexture(texurePaths[TextureType.Crate]);

        /* CAMERA MOVEMENT INPUT*/
        var camMovInputOptions = new KeyboardKey[4] { KeyboardKey.KEY_W, KeyboardKey.KEY_A, KeyboardKey.KEY_S, KeyboardKey.KEY_D, };

        Vector2 GetCamMoveSpeed(KeyboardKey key) =>
            key switch
            {
                KeyboardKey.KEY_W => new Vector2(0, -camMoveSpeed * Raylib.GetFrameTime()),
                KeyboardKey.KEY_A => new Vector2(-camMoveSpeed * Raylib.GetFrameTime(), 0),
                KeyboardKey.KEY_S => new Vector2(0, camMoveSpeed * Raylib.GetFrameTime()),
                KeyboardKey.KEY_D => new Vector2(camMoveSpeed * Raylib.GetFrameTime(), 0),
                _ => Vector2.Zero,
            };
        /* END CAMERA MOVEMENT INPUT*/

        while (!Raylib.WindowShouldClose())
        {
            mousePos = Raylib.GetMousePosition();

            /* CAMERA MOVEMENT*/
            foreach (var input in camMovInputOptions)
                if (Raylib.IsKeyDown(input))
                    target += GetCamMoveSpeed(input);

            cam.target = target;
            /* END CAMERA MOVEMENT*/

            /*Grapics*/
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.GRAY);

            Raylib.BeginMode2D(cam);

            /* DRAW BAKCGROUND */
            Raylib.DrawTexturePro(
                background,
                new Rectangle(0, 0, background.width, background.height),
                new Rectangle(target.X, target.Y, SCREEN_WIDTH, SCREEN_HEIGHT),
                Vector2.Zero,
                0,
                Color.WHITE);
            /* END DRAW BAKCGROUND */

            /* DRAW GRID */
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
            /* END DRAW GRID */

            /*UI*/
            var _onClick = () => Console.WriteLine("test");
            UI.btn(target, mousePos, "qeoirwqre", 10, 10, 300, 50, _onClick);
            UI.btn(target, mousePos, "qeoirwqre", 10, 60, 300, 50, _onClick);
            UI.btn(target, mousePos, "qeoirwqre", 10, 110, 300, 50, _onClick);

            Raylib.DrawFPS(Raylib.GetScreenWidth() - 100 + (int)(target.X), 10 + (int)target.Y);
            /*END UI*/

            Raylib.EndMode2D();
            Raylib.EndDrawing();
            /*End Grapics*/
        }

        Raylib.UnloadTexture(background);
        Raylib.UnloadTexture(crate);
        Raylib.CloseWindow();
    }
}

public static class UI
{
    public static void btn(Vector2 target, Vector2 mousePos, string text, int x, int y, int width, int height, Action onClick)
    {
        Color bgColor = Color.BLUE;
        Color exColor = Color.DARKBLUE;
        Color bgColorHover = Color.GREEN;
        Color exColorHover = Color.DARKGREEN;

        int _x = x + (int)target.X;
        int _y = y + (int)target.Y;
        var btnRec = new Rectangle(_x, _y, width, height);
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

        Raylib.DrawRectangle(_x, _y, width, height, btnCol);
        Raylib.DrawRectangleLinesEx(btnRec, 2, btnExCol);

        var textX = _x + (width / 4);
        var textY = _y + (height / 4);

        Raylib.DrawText(text, textX, textY, 32, Color.BLACK);
    }
}