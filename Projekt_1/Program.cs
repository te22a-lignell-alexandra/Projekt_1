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
// typ win = false om fienden lever och om win = false => skapa väggar framför vinst dörren eller nåt


using Raylib_cs;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

Raylib.InitWindow(800, 600, "screen");
Raylib.SetTargetFPS(60);





// CHARACTER STUFF
Texture2D characterImage = Raylib.LoadTexture("papillon.png");
Rectangle characterRect = new Rectangle(400, 300, 64, 64);
Vector2 movement = new Vector2(0, 0);

// ENEMY STUFF
Texture2D enemyImage = Raylib.LoadTexture("enemy-ghost.png");
Rectangle enemyRect = new Rectangle(400, 300, 64, 64);
Vector2 enemyMovement = new Vector2(0, 0);


// LISTS
List<Rectangle> doorsGreen = new();
doorsGreen.Add(new Rectangle(0, 150, 10, 100));
doorsGreen.Add(new Rectangle(600, 0, 100, 10));

List<Rectangle> doorsBlack = new();
doorsBlack.Add(new Rectangle(0, 450, 10, 100));

List<Rectangle> doorsPurple = new();
doorsPurple.Add(new Rectangle(790, 400, 10, 100));


List<Rectangle> walls = new();
walls.Add(new Rectangle(300, 0, 50, 200));
walls.Add(new Rectangle(0, 300, 350, 50));
walls.Add(new Rectangle(500, 0, 50, 300));

// VARIABLES
string scene = "start";
int hp = 3;
float speed = 5;
float enemySpeed = 2;



while (!Raylib.WindowShouldClose())
{
    if (scene == "start")
    {
        scene = Start(scene);
    }

    // GAME START------------------------------------------
    else if (scene != "start")
    {
        movement = Movement(movement, speed);


        // COLLISION WITH WALLS
        characterRect.X += movement.X;
        if (CollidesWithWalls(characterRect, walls)) characterRect.X -= movement.X;

        characterRect.Y += movement.Y;
        if (CollidesWithWalls(characterRect, walls)) characterRect.Y -= movement.Y;


        // COLLISION WITH EDGE OF SCREEN
        if (CollidesWithEdgeX(characterRect)) characterRect.X -= movement.X;
        if (CollidesWithEdgeY(characterRect)) characterRect.Y -= movement.Y;



        // DOORS-------------------------------------
        if (scene == "roomGreen")
        {
            if (Raylib.CheckCollisionRecs(characterRect, doorsGreen[0]))
            {
                scene = "roomPurple";
            }
            if (Raylib.CheckCollisionRecs(characterRect, doorsGreen[1]))
            {
                scene = "roomBlack";
            }
        }
        if (scene == "roomBlack")
        {
            enemyMovement = EnemyMovement(enemyMovement, enemyRect, enemySpeed);
            if (Raylib.CheckCollisionRecs(characterRect, doorsBlack[0]))
            {
                scene = "roomGreen";
            }
        }
        if (scene == "roomPurple")
        {
            if (Raylib.CheckCollisionRecs(characterRect, doorsPurple[0]))
            {
                scene = "End";
            }
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
    else if (scene == "roomGreen")
    {
        DrawRoom(characterImage, characterRect, doorsGreen, walls, hp, Color.GREEN, Color.GOLD, Color.DARKPURPLE);
    }
    // ROOM 2
    else if (scene == "roomBlack")
    {
        DrawRoom(characterImage, characterRect, doorsBlack, walls, hp, Color.BLACK, Color.WHITE, Color.GREEN);
        DrawEnemy(enemyImage, enemyRect);
    }
    // ROOM 3
    else if (scene == "roomPurple")
    {
        DrawRoom(characterImage, characterRect, doorsPurple, walls, hp, Color.DARKPURPLE, Color.BLACK, Color.WHITE);
    }


    // END SCENE
    else if (scene == "End")
    {
        DrawEndScene(Color.GOLD, Color.MAROON);
    }

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

static void DrawEnemy(Texture2D enemyImage, Rectangle enemyRect)
{
    Raylib.DrawTexture(enemyImage, (int)enemyRect.X, (int)enemyRect.Y, Color.WHITE);
}

static void DrawRoom(Texture2D characterImage, Rectangle characterRect, List<Rectangle> doors, List<Rectangle> walls, int hp, Color bkgColor, Color wallColor, Color doorColor)
{
    Raylib.ClearBackground(bkgColor);
    DrawHp(hp);

    foreach (Rectangle door in doors)
    {
        Raylib.DrawRectangleRec(door, doorColor);
    }

    foreach (Rectangle wall in walls)
    {
        Raylib.DrawRectangleRec(wall, wallColor);
    }

    DrawCharacter(characterImage, characterRect);
}


static void DrawEndScene(Color bkgColor, Color textColor)
{
    Raylib.ClearBackground(bkgColor);
    Raylib.DrawText("YOU WON!", 280, 270, 50, textColor);
}

// ---------------------------MOVEMENT-----------------------------------------
static Vector2 Movement(Vector2 movement, float speed)
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

    return movement;
}

static Vector2 EnemyMovement(Vector2 enemyMovement, Rectangle enemyRect, float enemySpeed)
{
    if(enemyRect.Y == 600 || enemyRect.Y == 400)
    {
        enemySpeed = enemySpeed*-1;
    }

    return enemyMovement;
}
// --------------------------------START SCREEN----------------------------
static string Start(string scene)
{
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
    {
        scene = "roomGreen";
    }

    return scene;
}