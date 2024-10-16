using System.Collections;
using System.Collections.Generic;
using Azul.Controller;
using Azul.Model;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Azul
{
    public class TestScoreBoardController
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void TestScoreForSingleTilePlacement(int space)
        {
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            int points = controller.CalculatePointsForTilePlacement(altar, space);
            Assert.AreEqual(1, points);
        }

        [Test]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public void TestScoreWhenPreviousAdjacentIsFilled(int testSpace)
        {
            int filledSpace = testSpace - 1;
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            this.FillSpace(altar, filledSpace, TileColor.RED);
            Assert.AreEqual(2, controller.CalculatePointsForTilePlacement(altar, testSpace));
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void TestScoreWhenNextAdjacentIsFilled(int testSpace)
        {
            int filledSpace = testSpace + 1;
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            this.FillSpace(altar, filledSpace, TileColor.RED);
            Assert.AreEqual(2, controller.CalculatePointsForTilePlacement(altar, testSpace));
        }

        [Test]
        public void TestScoreForwardWrapAroundSixToOne()
        {
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            this.FillSpace(altar, 1, TileColor.RED);
            Assert.AreEqual(2, controller.CalculatePointsForTilePlacement(altar, 6));
        }

        [Test]
        public void TestScoreWrapAroundComplete()
        {
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            this.FillSpace(altar, 1, TileColor.RED);
            this.FillSpace(altar, 2, TileColor.RED);
            this.FillSpace(altar, 3, TileColor.RED);
            this.FillSpace(altar, 5, TileColor.RED);
            this.FillSpace(altar, 6, TileColor.RED);
            Assert.AreEqual(6, controller.CalculatePointsForTilePlacement(altar, 4));
        }
        [Test]
        public void TestScoreBackwardWrapAroundOneToSix()
        {
            ScoreBoardController controller = this.CreateScoreBoardController();
            Altar altar = this.CreateEmptyAltar(TileColor.RED);
            this.FillSpace(altar, 6, TileColor.RED);
            Assert.AreEqual(2, controller.CalculatePointsForTilePlacement(altar, 1));
        }

        private ScoreBoardController CreateScoreBoardController()
        {
            GameObject gameObject = new("Score Board Controller");
            ScoreBoardController controller = gameObject.AddComponent<ScoreBoardController>();
            return controller;
        }

        private Altar CreateEmptyAltar(TileColor tileColor)
        {
            Altar altar = Altar.Create(tileColor);
            List<AltarSpace> spaces = new();
            for (int idx = 1; idx <= 6; idx++)
            {
                spaces.Add(AltarSpace.Create(tileColor, idx));
            }
            altar.AddAltarSpaces(spaces);
            return altar;
        }

        private void FillSpace(Altar altar, int value, TileColor fillColor)
        {
            AltarSpace space = altar.GetSpaces().Find(space => space.GetValue() == value);
            space.PlaceTile(fillColor);
        }
    }
}
