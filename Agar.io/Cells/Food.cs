﻿using Agario.GameLogic;
using Agario.GameLogic;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using System.Text.Json.Serialization;
using Timer = Agario.GameLogic.Timer;

namespace Agario.Cells
{
    sealed public class Food : Cell, IUpdatable
    {
        private Random _rand = new Random();
        private Cell? _target;
        public Cell? Target
        {
            get { return _target; }
            set { _target = value; }
        }
        public bool IsEaten { get; set; } = false;
        public Food(float mass = 30)
        {
            ChangePos(Game.MapSizeX, Game.MapSizeY);
            Mass = Logic.Random.Next(10, 50);
            Circle.FillColor = new Color((byte)Logic.Random.Next(0, 255), (byte)Logic.Random.Next(0, 255), (byte)Logic.Random.Next(0, 255));
            Circle.SetPointCount(20);
            IsEaten = false;
        }
        public void ChangePos(int sizeX, int sizeY)
        {
            Position = Logic.GenereatePoint(sizeX, sizeY);
        }
        public void Update(RenderWindow window)
        {
            if (IsEaten)
            {
                float dX = _target.X - X;
                float dY = _target.Y - Y;
                float distance = MathF.Sqrt(MathF.Pow(dX, 2) + MathF.Pow(dY, 2));
                var direction = new Vector2f(dX / distance, dY / distance);
                Position += (direction * (2 * Timer.DeltaTime * 100)) * 2;
                if (distance > Radius * 2) IsEaten = false;
            }
            else
            {
                _rand = new Random();
                float moveX = Logic.Random.Next(-39, 40) * Timer.DeltaTime;
                float moveY = Logic.Random.Next(-39, 40) * Timer.DeltaTime;
                if (Math.Abs(X + moveX) >= Game.MapSizeX)
                    moveX = 0;
                if (Math.Abs(Y + moveY) >= Game.MapSizeY)
                    moveY = 0;
                Position += new Vector2f(moveX, moveY);
            }
        }
    }
}
