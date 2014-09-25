using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Controllers
{
    public class FacadeController : IFacadeController
    {
        private static FacadeController instance = null;

        private User currentUser;
        private Round currentRound;
        private List<DTRanking> ranking;

        private FacadeController()
        {

        }

        public static FacadeController getInstance()
        {
            if (FacadeController.instance == null)
                FacadeController.instance = new FacadeController();

            return FacadeController.instance;
        }

        public bool isLoggedUser()
        {
            return this.currentUser != null;
        }

        public User getCurrentUser()
        {
            return this.currentUser;
        }

        public List<DTRanking> getRanking() 
        {
            return this.ranking;
        }

        public void loginUser(User user) 
        {
            this.currentUser = user;

            UsainBolt usainBolt = new UsainBolt();

            Round round = new Round();
            round.RoundId = 1;
            round.StartDate = new DateTime(2014, 09, 22, 0, 0, 0);
            round.EndDate = new DateTime(2014, 09, 29, 0, 0, 0);
            round.Challenges = new Dictionary<int, Challenge>();
            round.Challenges.Add(usainBolt.ChallengeId, usainBolt);

            usainBolt.Round = round;

            State usainBoltState = new State();

            usainBolt.State = usainBoltState;
            usainBoltState.setChallenge(usainBolt);

            this.currentRound = round;

            DTRanking r1 = new DTRanking(this.currentUser.UserId, 1, 1280, this.currentUser.FirstName + " " + this.currentUser.LastName, this.currentUser.ImageUrl);
            DTRanking r2 = new DTRanking(2, 2, 1270, "Martín Berguer", "http://graph.facebook.com/100002316914037/picture?type=square");
            DTRanking r3 = new DTRanking(3, 3, 1260, "Cristian Bauza", "http://graph.facebook.com/cristian.bauza/picture");
            DTRanking r4 = new DTRanking(4, 4, 1240, "Pablo Olivera", "http://graph.facebook.com/pablo.olivera/picture");
            DTRanking r5 = new DTRanking(5, 5, 1230, "Alejandro Brusco", "http://graph.facebook.com/alejandro.brusco/picture?type=square");
            DTRanking r6 = new DTRanking(6, 6, 1220, "Felipe Garcia", "http://graph.facebook.com/felipe92/picture?type=square");
            DTRanking r7 = new DTRanking(7, 7, 1210, "Martín Steglich", "http://graph.facebook.com/tinchoste/picture?type=square");

            this.ranking = new List<DTRanking>();
            this.ranking.Add(r1);
            this.ranking.Add(r2);
            this.ranking.Add(r3);
            this.ranking.Add(r4);
            this.ranking.Add(r5);
            this.ranking.Add(r6);
            this.ranking.Add(r7);
        }

        public void logoutUser()
        {
            this.currentUser = null;
            this.currentRound = null;
        }
    }
}
