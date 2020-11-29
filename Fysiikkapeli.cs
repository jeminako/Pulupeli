using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

// @author Jemina Kotkajuuri
// @version 29.11.2020
public class Pulupeli : PhysicsGame
{
    private const int TALOJEN_MAARA = 10;
    private Timer aikaLaskuri;
    private EasyHighScore topLista = new EasyHighScore();
    private double edellisenTalonReuna;
    private PlatformCharacter pulu = null;


    /// <summary>
    /// Aloittaa pelin.
    /// </summary>
    public override void Begin()
    {
        LuoTausta();
        Alkuruutu();
    }


    /// <summary>
    /// Luo itse pelin
    /// </summary>
    private void LuoPeli()
    {
        LuoAikaLaskuri();
        LuoPulu();
        LuoEnsimmäinenTalo();
        LuoTaloja();

        Gravity = new Vector(0.0, -800.0);
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Luo alkuvalikon
    /// </summary>
    private void Alkuruutu()
    {
        MultiSelectWindow alku = new MultiSelectWindow("Pulupeli", "Aloita peli", "Top-pelaajat", "Lopeta");
        alku.AddItemHandler(0, LuoPeli);
        alku.AddItemHandler(1, Pistetilasto);
        alku.AddItemHandler(2, Exit);
        Add(alku);

    }


    /// <summary>
    /// Näyttää pistetilaston
    /// </summary>
    private void Pistetilasto()
    {
        topLista.Show();
        topLista.HighScoreWindow.Closed += AloitaPeli;
    }


    /// <summary>
    /// Luo pelin taustan.
    /// </summary>
    private void LuoTausta()
    {
        GameObject tausta = new GameObject(Screen.Width, Screen.Height);
        tausta.Image = LoadImage("tausta2");
        Add(tausta, -3);
        Layers[-3].RelativeTransition = new Vector(0.0, 0.0);
    }


    /// <summary>
    /// Luo pelattavan hahmon.
    /// </summary>
    private void LuoPulu()
    {
        pulu = new PlatformCharacter(60.0, 60.0);
        pulu.Shape = Shape.Circle;
        pulu.Color = Jypeli.Color.Gray;
        pulu.X = 0;
        pulu.Y = Level.Bottom + 430;
        pulu.Image = LoadImage("pulu2");
        pulu.LinearDamping = 1;
        pulu.KineticFriction = 0.0;
        pulu.StaticFriction = 0.0;
        pulu.CanRotate = false;

        Add(pulu);
        

        Camera.Follow(pulu);

        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppy, "Hyppää", pulu, 300.0);
    }


    /// <summary>
    /// Pävittää pelin 60 kertaa sekunnissa.
    /// </summary>
    /// <param name="time">Pelin aika</param>
    protected override void Update(Time time)
    {
        base.Update(time);
        if (pulu != null)
        {
            pulu.Velocity = new Vector(500, pulu.Velocity.Y);
            if (!pulu.IsDestroyed && pulu.Y < Level.Bottom + 350)
            {
                PuluKuoli();
            }
        }
    }


    /// <summary>
    /// Luo pelin aloituspaikan.
    /// </summary>
    private void LuoEnsimmäinenTalo()
    {
        LuoTalo(0);
    }


    /// <summary>
    /// Luo 10 taloa.
    /// </summary>
    private void LuoTaloja()
    {
        double vali = 380;
        for (int i = 0; i < TALOJEN_MAARA; i++)
        {
            LuoTalo(edellisenTalonReuna + vali);
        }

        Timer ajastin = new Timer();
        ajastin.Interval = 14;
        ajastin.Timeout += delegate { LuoTaloja(); };
        ajastin.Start();
    }


    /// <summary>
    /// Luo talon
    /// </summary>
    /// <param name="x">Talon paikan x-koordinaatti</param>
    private void LuoTalo(double x)
    {
        PhysicsObject talo = new PhysicsObject(RandomGen.NextDouble(300.0, 600.0), 800.0);
        talo.X = x + (talo.Width / 2);
        edellisenTalonReuna = talo.X + (talo.Width / 2);
        talo.Y = Level.Bottom;
        talo.IgnoresGravity = true;
        talo.CanRotate = false;
        talo.KineticFriction = 0.0;
        talo.StaticFriction = 0.0;
        talo.MaximumLifetime = TimeSpan.FromSeconds(400);
        Add(talo);
        talo.MakeStatic();
    }


    /// <summary>
    /// Luo aikalaskurin.
    /// </summary>
    private void LuoAikaLaskuri()
    {
        aikaLaskuri = new Timer();
        aikaLaskuri.Start();

        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.Black;
        aikaNaytto.DecimalPlaces = 1;
        aikaNaytto.X = 300;
        aikaNaytto.Y = 300;
        aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
        Add(aikaNaytto);
    }


    /// <summary>
    /// Aliohjelma, jossa peli lopetetaan.
    /// </summary>
    private void PuluKuoli()
    {
        pulu.Destroy();
        topLista.EnterAndShow(Math.Round(aikaLaskuri.SecondCounter.Value, 2));
        aikaLaskuri.Stop();
        topLista.HighScoreWindow.Closed += AloitaPeli;
    }


    /// <summary>
    /// Aliohjelma, joka aloittaa pelin uudestaan.
    /// </summary>
    /// <param name="sender">Ikkuna.</param>
    private void AloitaPeli(Window sender)
    {
        ClearAll();
        Begin();
    }


    /// <summary>
    /// Aliohjelma hyppäämiseen.
    /// </summary>
    /// <param name="pulu">Hyppäävä hahmo.</param>
    /// <param name="impulssi">Voima, jolla hypätään</param>
    private void Hyppy(PlatformCharacter pulu, double impulssi)
    {
        pulu.Jump(impulssi);
    }
}
