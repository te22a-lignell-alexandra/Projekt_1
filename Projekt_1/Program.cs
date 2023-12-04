// ändra hastigheten m *-1 för att flytta fiender fram och tillbaka mellan två punkter

// om fiendePos.x = 300 eller om fiendePos.x = 100: (300 och 100 blir vändpunkterna)
//      fiendeMovement = fiendeMovement*-1

// (så blir det negativt och fienden byter riktning, +*-=- ,-*-=+)

// variabel för start, om träffar fiende --> position = start (för karaktär och fiende) NOPE

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
Vector2 enemyMovement = new Vector2(0, 6);

// WEAPON STUFF
Texture2D weaponImage = Raylib.LoadTexture("flame-sword.png");
Rectangle weaponRect = new Rectangle(50, 100, 64, 64);


// LISTS
List<Rectangle> doors = new();
// green room purple, black
doors.Add(new Rectangle(0, 150, 10, 100));
doors.Add(new Rectangle(600, 0, 100, 10));
// black room, green
doors.Add(new Rectangle(0, 450, 10, 100));
// purple room, gold
doors.Add(new Rectangle(790, 400, 10, 100));
// Bättre sätt att göra dörrarna?
// ?????????????????????????????????????????????????????????????
// ?????????????????????????????????????????????????????????????

List<Rectangle> walls = new();
walls.Add(new Rectangle(300, 0, 50, 200));
walls.Add(new Rectangle(0, 300, 350, 50));
walls.Add(new Rectangle(500, 0, 50, 300));

List<Rectangle> wallBlock = new();
wallBlock.Add(new Rectangle(750, 350, 50, 200));

// VARIABLES
string scene = "start";
int hp = 3;
float speed = 5;
bool isWeaponPickedUp = false;
bool isEnemyAlive = true;







while (!Raylib.WindowShouldClose())
{

    if (scene == "start") scene = Start(scene);



    // GAME START------------------------------------------
    else if (scene != "start")
    {
        movement = Vector2.Zero;
        movement = Movement(movement, speed);


        // COLLISION WITH WALLS
        characterRect.X += movement.X;
        if (CollidesWithWalls(characterRect, walls)) characterRect.X -= movement.X;

        characterRect.Y += movement.Y;
        if (CollidesWithWalls(characterRect, walls)) characterRect.Y -= movement.Y;

        if (isEnemyAlive == true)
        {
            if(CollidesWithWalls(characterRect, wallBlock)) characterRect.X -= movement.X;
            if(CollidesWithWalls(characterRect, wallBlock)) characterRect.Y -= movement.Y;
        }

        // COLLISION WITH EDGE OF SCREEN
        if (CollidesWithEdgeX(characterRect)) characterRect.X -= movement.X;
        if (CollidesWithEdgeY(characterRect)) characterRect.Y -= movement.Y;


        // DIFFERENT ROOMS-------------------------------------
        if (scene == "roomGreen")
        {
            if (Raylib.CheckCollisionRecs(characterRect, doors[0])) scene = "roomPurple";
            if (Raylib.CheckCollisionRecs(characterRect, doors[1])) scene = "roomBlack";
        } 

        if (scene == "roomBlack")
        {
            if (Raylib.CheckCollisionRecs(characterRect, weaponRect)) isWeaponPickedUp = true;


            enemyRect.Y += enemyMovement.Y;
            if (enemyRect.Y >= 600 - enemyRect.Height || enemyRect.Y <= 0) enemyMovement.Y *= -1;

            if (Raylib.CheckCollisionRecs(characterRect, enemyRect)) 
            {
                if (isWeaponPickedUp == false)
                {
                    hp -= 1;
                    characterRect.X = 600;
                    characterRect.Y = 0;
                }
                if (isWeaponPickedUp == true)
                {
                    isEnemyAlive = false;
                }
            }

            if (Raylib.CheckCollisionRecs(characterRect, doors[2])) scene = "roomGreen";

        }
        if (scene == "roomPurple")
        {
            if (Raylib.CheckCollisionRecs(characterRect, doors[2])) scene = "roomGreen";
            if (Raylib.CheckCollisionRecs(characterRect, doors[3])) scene = "Win";
        }
        if (hp == 0) scene = "GameOver";
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
        DrawRoom(characterImage, characterRect, doors, walls, hp, Color.GREEN, Color.GOLD);
        Raylib.DrawRectangleRec(doors[0], Color.DARKPURPLE);
        Raylib.DrawRectangleRec(doors[1], Color.BLACK);
    }

    // ROOM 2
    else if (scene == "roomBlack")
    {
        DrawRoom(characterImage, characterRect, doors, walls, hp, Color.BLACK, Color.WHITE);
        Raylib.DrawRectangleRec(doors[2], Color.GREEN);


        DrawWeapon(weaponImage, weaponRect, isWeaponPickedUp);
        if (isEnemyAlive == true) DrawEnemy(enemyImage, enemyRect);
    }

    // ROOM 3
    else if (scene == "roomPurple")
    {
        DrawRoom(characterImage, characterRect, doors, walls, hp, Color.DARKPURPLE, Color.BLACK);
        if(isEnemyAlive == true) Raylib.DrawRectangleRec(wallBlock[0], Color.RED);
        
        Raylib.DrawRectangleRec(doors[2], Color.GREEN);
        Raylib.DrawRectangleRec(doors[3], Color.GOLD);
    }

    // END SCENES
    else if (scene == "Win") DrawWinScene(Color.GOLD, Color.MAROON);
    else if (scene == "GameOver") DrawGameOverScene(Color.MAROON, Color.GOLD);

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

static void DrawWeapon(Texture2D weaponImage, Rectangle weaponRect, bool isWeaponPickedUp)
{
    if (isWeaponPickedUp == false)
    {
        Raylib.DrawTexture(weaponImage, (int)weaponRect.X, (int)weaponRect.Y, Color.WHITE);
    }
}

static void DrawRoom(Texture2D characterImage, Rectangle characterRect, List<Rectangle> doors, List<Rectangle> walls, int hp, Color bkgColor, Color wallColor)
{
    Raylib.ClearBackground(bkgColor);
    DrawHp(hp);

    // foreach (Rectangle door in doors)
    // {
    //     Raylib.DrawRectangleRec(door, doorColor);
    // }

    foreach (Rectangle wall in walls)
    {
        Raylib.DrawRectangleRec(wall, wallColor);
    }

    DrawCharacter(characterImage, characterRect);
}


static void DrawWinScene(Color bkgColor, Color textColor)
{
    Raylib.ClearBackground(bkgColor);
    Raylib.DrawText("YOU WON!", 280, 270, 50, textColor);
}

static void DrawGameOverScene(Color bkgColor, Color textColor)
{
    Raylib.ClearBackground(bkgColor);
    Raylib.DrawText("GAME OVER", 280, 270, 50, textColor);
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
        scene = "roomGreen";
    }

    return scene;
}