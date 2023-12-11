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


// Timer????

// Lägg till köttbit som ger ett hp

using Raylib_cs;
using System.ComponentModel;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;

Raylib.InitWindow(800, 600, "screen");
Raylib.SetTargetFPS(60);



// VARIABLES
string scene = "start";
int hp = 3;
float speed = 5;
bool isWeaponPickedUp = false;
bool isGhostAlive = true;
bool isSpiderAlive = true;



// CHARACTER STUFF
Texture2D characterImage = Raylib.LoadTexture("papillon.png");
Rectangle characterRect = new Rectangle(400, 300, 64, 64);
Vector2 movement = new Vector2(0, 0);

// ENEMY STUFF
Texture2D ghostImage = Raylib.LoadTexture("enemy-ghost.png");
Rectangle ghostRect = new Rectangle(400, 500, 64, 64);
Vector2 ghostMovement = new Vector2(0, 6);

Texture2D spiderImage = Raylib.LoadTexture("poison-spider.png");
Rectangle spiderRect = new Rectangle(400, 450, 64, 64);
Vector2 spiderMovement = new Vector2(5, 0);
// LADDA NER STÖRRE VERSION AV SPINDELN


// WEAPON STUFF
Texture2D weaponImage = Raylib.LoadTexture("flame-sword.png");
Rectangle weaponRect = new Rectangle(50, 100, 64, 64);



// LISTS--------------------------------------------
List<Rectangle> doors = new();
// green room purple, black
doors.Add(new Rectangle(0, 150, 10, 100));
doors.Add(new Rectangle(600, 0, 100, 10));
// black room, green
doors.Add(new Rectangle(0, 450, 10, 100));
// purple room, gold
doors.Add(new Rectangle(790, 400, 10, 100));
// Bättre sätt att göra dörrarna? Klass?
// ?????????????????????????????????????????????????????????????
// ?????????????????????????????????????????????????????????????

List<Rectangle> walls = new();
walls.Add(new Rectangle(300, 0, 50, 200));
walls.Add(new Rectangle(0, 300, 350, 50));
walls.Add(new Rectangle(500, 0, 50, 300));

List<Rectangle> wallBlock = new();
wallBlock.Add(new Rectangle(750, 350, 50, 200));
// -------------------------------------------------------------







while (!Raylib.WindowShouldClose())
{
    // START SCENE
    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE)) scene = "roomGreen";

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
        
        // red wall
        if ((isGhostAlive == true || isSpiderAlive == true) && scene == "roomPurple")
        {
            if (CollidesWithWalls(characterRect, wallBlock)) characterRect.X -= movement.X;
            if (CollidesWithWalls(characterRect, wallBlock)) characterRect.Y -= movement.Y;
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

        if (scene == "roomBlack") RoomBlack(ref scene, ref hp, ref isWeaponPickedUp, ref isGhostAlive, ref characterRect, ref ghostRect, ref ghostMovement, weaponRect, doors);
        if (scene == "roomPurple") RoomPurple(ref scene, ref hp, isWeaponPickedUp, ref isSpiderAlive, ref characterRect, ref spiderRect, ref spiderMovement, doors);
        // How to loose the game
        if (hp == 0) scene = "GameOver";
    }

    // -----------------------------------------------------------------------------
    // DRAW

    Raylib.BeginDrawing();
    // Rita ut alla olika rum
    if (scene == "start") DrawStartScene();

    else if (scene == "roomGreen") DrawRoomGreen(hp, characterImage, characterRect, doors, walls);

    else if (scene == "roomBlack") DrawRoomBlack(hp, isWeaponPickedUp, isGhostAlive, characterImage, characterRect, ghostImage, ghostRect, weaponImage, weaponRect, doors, walls);

    else if (scene == "roomPurple") DrawRoomPurple(hp, isGhostAlive, isSpiderAlive, characterImage, characterRect, spiderImage, spiderRect, doors, walls, wallBlock);

    else if (scene == "Win") DrawEndScene(Color.GOLD, Color.MAROON, "YOU WON!");

    else if (scene == "GameOver") DrawEndScene(Color.MAROON, Color.GOLD, "GAME OVER!");

    Raylib.EndDrawing();
}



// ------------------------------------------------------------------------------------
// METHODS
// ------------------------------------------------------------------------------------


// ------------------------------------COLLISIONS-------------------------------------------

// Character collides with enemy: Either loose hp and teleport or, if weapon, kill enemy
static void EnemyCollision(ref int hp, bool isWeaponPickedUp, ref bool isEnemyAlive, ref Rectangle characterRect)
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

// Character can't run outside of screen (Y direction)
bool CollidesWithEdgeY(Rectangle characterRect)
{
    if (characterRect.Y > 600 - characterRect.Height || characterRect.Y < 0)
    {
        return true;
    }

    return false;
}

// Character can't run outside of the screen (X direction)
bool CollidesWithEdgeX(Rectangle characterRect)
{
    if (characterRect.X > 800 - characterRect.Width || characterRect.X < 0)
    {
        return true;
    }

    return false;
}

// Character can't run through walls
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


// -----------------------------------DRAW SMALL THINGS-----------------------------------

// Display HP in the top left corner
static void DrawHp(int hp)
{
    Raylib.DrawText($"HP: {hp}", 20, 20, 40, Color.WHITE);
}

// Draw the player character
static void DrawCharacter(Texture2D characterImage, Rectangle characterRect)
{
    Raylib.DrawTexture(characterImage, (int)characterRect.X, (int)characterRect.Y, Color.WHITE);
}

// Draw out an enemy (used for both)
static void DrawEnemy(Texture2D enemyImage, Rectangle enemyRect)
{
    Raylib.DrawTexture(enemyImage, (int)enemyRect.X, (int)enemyRect.Y, Color.WHITE);
}

// Draw out the sword, unless it's picked up
static void DrawWeapon(Texture2D weaponImage, Rectangle weaponRect, bool isWeaponPickedUp)
{
    if (isWeaponPickedUp == false)
    {
        Raylib.DrawTexture(weaponImage, (int)weaponRect.X, (int)weaponRect.Y, Color.WHITE);
        Raylib.DrawText("Monster-slaying sword", 20, 180, 15, Color.RED);
    }
}

// Draw the text shown in the beginning
static void DrawStartText()
{
    Raylib.DrawText("THE GAME", 250, 200, 50, Color.WHITE);
    Raylib.DrawText("Use W,A,S,D to move around.", 250, 300, 20, Color.GOLD);
    Raylib.DrawText("A door's color is the same as the color of the room it leads to.", 60, 350, 20, Color.GOLD);
    Raylib.DrawText("Kill the monsters to unlock the blocked door", 120, 400, 25, Color.RED);
    Raylib.DrawText("Press SPACE to start", 250, 500, 25, Color.WHITE);
}


// --------------------------------DRAW SCENES/ROOMS----------------------------
    
// Draw out the basic things in a room (base for all rooms)
static void DrawRoom(Texture2D characterImage, Rectangle characterRect, List<Rectangle> doors, List<Rectangle> walls, int hp, Color bkgColor, Color wallColor)
{
    Raylib.ClearBackground(bkgColor);
    DrawHp(hp);
    foreach (Rectangle wall in walls)
    {
        Raylib.DrawRectangleRec(wall, wallColor);
    }

    DrawCharacter(characterImage, characterRect);
}

// Draw out everything in the purple room
static void DrawRoomPurple(int hp, bool isGhostAlive, bool isSpiderAlive, Texture2D characterImage, Rectangle characterRect, Texture2D spiderImage, Rectangle spiderRect, List<Rectangle> doors, List<Rectangle> walls, List<Rectangle> wallBlock)
{
    DrawRoom(characterImage, characterRect, doors, walls, hp, Color.DARKPURPLE, Color.BLACK);
   
    if (isGhostAlive == true || isSpiderAlive == true) Raylib.DrawRectangleRec(wallBlock[0], Color.RED);
    if (isSpiderAlive == true) DrawEnemy(spiderImage, spiderRect);

    Raylib.DrawRectangleRec(doors[2], Color.GREEN);
    Raylib.DrawRectangleRec(doors[3], Color.GOLD);
}

//  Draw out everything in the black room
static void DrawRoomBlack(int hp, bool isWeaponPickedUp, bool isGhostAlive, Texture2D characterImage, Rectangle characterRect, Texture2D ghostImage, Rectangle ghostRect, Texture2D weaponImage, Rectangle weaponRect, List<Rectangle> doors, List<Rectangle> walls)
{
    DrawRoom(characterImage, characterRect, doors, walls, hp, Color.BLACK, Color.WHITE);
    Raylib.DrawRectangleRec(doors[2], Color.GREEN);


    DrawWeapon(weaponImage, weaponRect, isWeaponPickedUp);
    if (isGhostAlive == true) DrawEnemy(ghostImage, ghostRect);
}

// Draw out everything in the green room
static void DrawRoomGreen(int hp, Texture2D characterImage, Rectangle characterRect, List<Rectangle> doors, List<Rectangle> walls)
{
    DrawRoom(characterImage, characterRect, doors, walls, hp, Color.GREEN, Color.GOLD);
    Raylib.DrawRectangleRec(doors[0], Color.DARKPURPLE);
    Raylib.DrawRectangleRec(doors[1], Color.BLACK);
}

// Draw the start scene
static void DrawStartScene()
{
    Raylib.ClearBackground(Color.BLACK);
    DrawStartText();
}

// Draw win/game over scene (used for both)
static void DrawEndScene(Color bkgColor, Color textColor, string text)
{
    Raylib.ClearBackground(bkgColor);
    Raylib.DrawText(text, 280, 270, 50, textColor);
}


// ---------------------------MOVEMENT-----------------------------------------

// Character movement
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

// Spider enemy movement
static void SpiderMovement(ref Rectangle spiderRect, ref Vector2 spiderMovement)
{
    spiderRect.X += spiderMovement.X;
    if (spiderRect.X >= 600 - spiderRect.Height || spiderRect.X <= spiderRect.Width) spiderMovement.X *= -1;
}

// Ghost enemy movement
static void GhostMovement(ref Rectangle ghostRect, ref Vector2 ghostMovement)
{
    ghostRect.Y += ghostMovement.Y;
    if (ghostRect.Y >= 600 - ghostRect.Height || ghostRect.Y <= 0) ghostMovement.Y *= -1;
}


// ------------------------ALL THE CODE THAT ISN'T DRAWING RELATED FOR EACH ROOM-------------------------

// Code the purple room
static void RoomPurple(ref string scene, ref int hp, bool isWeaponPickedUp, ref bool isSpiderAlive, ref Rectangle characterRect, ref Rectangle spiderRect, ref Vector2 spiderMovement, List<Rectangle> doors)
{
    if (Raylib.CheckCollisionRecs(characterRect, doors[2])) scene = "roomGreen";
    if (Raylib.CheckCollisionRecs(characterRect, doors[3])) scene = "Win";

    // spider movement + collision
    SpiderMovement(ref spiderRect, ref spiderMovement);

    if (Raylib.CheckCollisionRecs(characterRect, spiderRect)) 
    {
        EnemyCollision(ref hp, isWeaponPickedUp, ref isSpiderAlive, ref characterRect);
    }
}

// Code the black room
static void RoomBlack(ref string scene, ref int hp, ref bool isWeaponPickedUp, ref bool isGhostAlive, ref Rectangle characterRect, ref Rectangle ghostRect, ref Vector2 ghostMovement, Rectangle weaponRect, List<Rectangle> doors)
{
    if (Raylib.CheckCollisionRecs(characterRect, doors[2])) scene = "roomGreen";
    if (Raylib.CheckCollisionRecs(characterRect, weaponRect)) isWeaponPickedUp = true;

    // ghost movement + collision
    GhostMovement(ref ghostRect, ref ghostMovement);
    if (Raylib.CheckCollisionRecs(characterRect, ghostRect)) EnemyCollision(ref hp, isWeaponPickedUp, ref isGhostAlive, ref characterRect);
}