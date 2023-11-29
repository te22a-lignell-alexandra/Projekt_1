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

// weapon, om man inte hämtar vapnet tar man skada av fienden, gdsjkf = false, om man tar den dödar
// man fienden och tar inte skada, giufjkgr = true. ??- man måste döda fienden för att kunna vinna??


using Raylib_cs;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

Raylib.InitWindow(800, 600, "screen");
Raylib.SetTargetFPS(60);






Texture2D characterImage = Raylib.LoadTexture("papillon.png");
Rectangle characterRect = new Rectangle(400, 300, 64, 64);
Vector2 movement = new Vector2(0, 0);


// LISTS
List<Rectangle> doors = new();
doors.Add(new Rectangle(0, 150, 10, 100));


List<Rectangle> walls = new();
walls.Add(new Rectangle(300, 0, 50, 200));
walls.Add(new Rectangle(0, 300, 350, 50));
walls.Add(new Rectangle(500, 0, 50, 300));


// VARIABLES
string scene = "start";
int hp = 3;
float speed = 5;



while (!Raylib.WindowShouldClose())
{
    if (scene == "start")
    {
        scene = Start(scene);
    }

    
    else if (scene != "start")
    {
        movement = Vector2.Zero;
        movement = Movement(movement, speed);


        // CANT MOVE OUTSIDE OF SCREEN OR THROUGH WALLS
        characterRect.X += movement.X;
        if (CollidesWithWalls(characterRect, walls)) characterRect.X -= movement.X;

        characterRect.Y += movement.Y;
        if (CollidesWithWalls(characterRect, walls)) characterRect.Y -= movement.Y;

        if (CollidesWithEdgeX(characterRect)) characterRect.X -= movement.X;
        if (CollidesWithEdgeY(characterRect)) characterRect.Y -= movement.Y;

        // DOORS
        if (Raylib.CheckCollisionRecs(characterRect, doors[0]))
        {
            scene = "lilaRum";
        }
    }


    // -----------------------------------------------------------------------------
    // DRAW
    // -----------------------------------------------------------------------------


    Raylib.BeginDrawing();
 
    if (scene == "start")
    {
        Raylib.ClearBackground(Color.BLACK);
        DrawStartText();
    }

    // ROOM 1
    else if (scene == "gröntRum")
    {
        DrawRoom(characterImage, characterRect, doors, walls, hp, Color.GREEN, Color.GOLD, Color.DARKPURPLE);
    }
    // ROOM 2
    else if (scene == "lilaRum")
    {
        DrawRoom(characterImage, characterRect, doors, walls, hp, Color.DARKPURPLE, Color.BLACK, Color.WHITE);
    }
    // För olika dörrar eller väggar synliga i olika rum??

    Raylib.EndDrawing();
}



// ------------------------------------------------------------------------------------
// METHODS


// ------------------------------------COLLISIONS-------------------------------------------
bool CollidesWithEdgeY(Rectangle characterRect)
{
    if (characterRect.Y > 600 - characterRect.Height || characterRect.Y < 0)
        {
            return true;
        }

    return false;
}

bool CollidesWithEdgeX(Rectangle characterRect)
{
    if (characterRect.X > 800 - characterRect.Width || characterRect.X < 0)
        {
            return true;
        }

    return false;
}

bool CollidesWithWalls(Rectangle characterRect, List<Rectangle> walls)
{
    foreach (Rectangle wall in walls)
    {
        if (Raylib.CheckCollisionRecs(characterRect, wall))
        {
            return true;
        }
    }

    return false;
}

// -----------------------------------DRAW-----------------------------------
static void DrawHp(int hp)
{
    Raylib.DrawText($"HP: {hp}", 20, 20, 30, Color.WHITE);
}

static void DrawStartText()
{
    Raylib.DrawText("Press SPACE to start", 200, 300, 30, Color.WHITE);
}

static void DrawCharacter(Texture2D characterImage, Rectangle characterRect)
{
    Raylib.DrawTexture(characterImage, (int)characterRect.X, (int)characterRect.Y, Color.WHITE);
}

static void DrawRoom(Texture2D characterImage, Rectangle characterRect, List<Rectangle> doors, List<Rectangle> walls, int hp, Color bkgColor, Color wallColor, Color doorColor)
{
    Raylib.ClearBackground(bkgColor);
    DrawHp(hp);
    DrawCharacter(characterImage, characterRect);

    foreach (Rectangle wall in walls)
    {
        Raylib.DrawRectangleRec(wall, wallColor);
    }

    foreach (Rectangle door in doors)
    {
        Raylib.DrawRectangleRec(door, doorColor);
    }
}

// ---------------------------MOVEMENT-----------------------------------------
static Vector2 Movement(Vector2 movement, float speed)
{
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

    return movement;
}


// --------------------------------START SCREEN----------------------------
static string Start(string scene)
{
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
    {
        scene = "gröntRum";
    }

    return scene;
}