// ändra hastigheten m *-1 för att flytta fiender fram och tillbaka mellan två punkter

// om fiendePos.x = 300 eller om fiendePos.x = 100: (300 och 100 blir vändpunkterna)
//      fiendeMovement = fiendeMovement*-1

// (så blir det negativt och fienden byter riktning, +*-=- ,-*-=+)


// variabel för start, om träffar fiende --> position = start (för karaktär och fiende)

// rörelse med knapptryck räknas som input

// Karaktär i labyrint, man kan inte gå igenom väggar, finns en fiende (minst)
// i varje rum, man har tre liv, vid kollision m fiende liv-1 och pos = start, 
// dörr rektanglar vid utgångar av labyrint som ändrar scen så det ser ut som att
// man går genom korridoren till nästa rum liksom. Pokal i man hämtar i slutet 
// eller nåt för att vinna.


using Raylib_cs;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

Raylib.InitWindow(800, 600, "screen");
Raylib.SetTargetFPS(60);


Texture2D characterImage = Raylib.LoadTexture("papillon.png");
Rectangle characterRect = new Rectangle(400, 300, 64, 64);
Vector2 movement = new Vector2(0,0);

// -----------------------------------------
// doors
List<Rectangle> doors = new();
doors.Add(new Rectangle (0, 150, 10, 100));

// -----------------------------------------
// WALLS
List<Rectangle> walls = new();
walls.Add(new Rectangle (300, 0, 50, 200));
walls.Add(new Rectangle (0, 300, 350, 50));
walls.Add(new Rectangle (500, 0, 50, 300));
// -----------------------------------------

string scene = "start";
int hp = 3;
float speed = 5;



while (!Raylib.WindowShouldClose())
{
    // START SCREEN
    if (scene == "start")
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            scene = "gröntRum";
        }
    }
    // MOVEMENT
    else if (scene !="start")
    {
        movement = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            movement.X = -1;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            movement.X = 1;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
        {
            movement.Y = -1;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            movement.Y = 1;
        }
        if (movement.Length() > 0)
        {
            movement = Vector2.Normalize(movement) * speed;
        }

        // CANT MOVE OUTSIDE OF SCREEN OR THROUGH WALLS
        // --------------------------------------------------------------------------------
        characterRect.X += movement.X;
        foreach (Rectangle wall in walls)
        {
            if (Raylib.CheckCollisionRecs(characterRect, wall))
            {
                characterRect.X -= movement.X;
            }
        }

        characterRect.Y += movement.Y;
        foreach (Rectangle wall in walls)
        {
            if (Raylib.CheckCollisionRecs(characterRect, wall))
            {
                characterRect.Y -= movement.Y;
            }
        }
        if (characterRect.X > 800 - characterRect.Width || characterRect.X < 0)
            {
                characterRect.X -= movement.X;
            }
        if (characterRect.Y > 600 - characterRect.Height || characterRect.Y < 0)
            {
                characterRect.Y -= movement.Y;
            }
        // DÖRRAR
        // -------------------------------------------------------------------------
        if (Raylib.CheckCollisionRecs(characterRect, doors[0]))
        {
            scene = "lilarum";
        }
    }
    // -----------------------------------------------------------------------------
    // DRAW
    // -----------------------------------------------------------------------------
    Raylib.BeginDrawing();
    // START SCREEN
        if (scene == "start")
        {
            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawText("Press SPACE to start", 200, 300, 30, Color.WHITE);
        }
        // RUM 1
        else if (scene == "gröntRum")
        {
            Raylib.ClearBackground(Color.GREEN);
            Raylib.DrawText($"HP: {hp}", 20, 20, 30, Color.WHITE);
            Raylib.DrawTexture(characterImage, (int)characterRect.X, (int)characterRect.Y, Color.WHITE);
            
            foreach (Rectangle wall in walls)
            {
                Raylib.DrawRectangleRec(wall, Color.GOLD);
            }
            foreach (Rectangle door in doors)
            {
                Raylib.DrawRectangleRec(door, Color.DARKPURPLE);
            }
        }
        else if (scene == "lilarum")
        {
            Raylib.ClearBackground(Color.DARKPURPLE);
            Raylib.DrawText($"HP: {hp}", 20, 20, 30, Color.WHITE);
            Raylib.DrawTexture(characterImage, (int)characterRect.X, (int)characterRect.Y, Color.WHITE);
            
            foreach (Rectangle wall in walls)
            {
                Raylib.DrawRectangleRec(wall, Color.BLACK);
            }
        }
    Raylib.EndDrawing();
}