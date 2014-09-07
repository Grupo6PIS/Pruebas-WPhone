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
using BeatIt_.AppCode.Enums;

namespace BeatIt_.AppCode.Datatypes    
{
    public class DTChallenge
    {
        private String challengeId { get; set; }
        private String challengeName { get; set; }
        private String challengeDescription { get; set; }
        private int challengeDuration { get; set; }
        private int challengeLevel { get; set; }
        private TState.STATE challengeState { get; set; }

        public DTChallenge(String challengeId,
                           String challengeName,
                           String challengeDescription,
                           int challengeDuration,
                           int challengeLevel,
                           TState.STATE challengeState)
        {
            this.challengeId = challengeId;
            this.challengeName = challengeName;
            this.challengeDescription = challengeDescription;
            this.challengeDuration = challengeDuration;
            this.challengeLevel = challengeLevel;
            this.challengeState = challengeState;
        }


        public String getChallengeId() { return this.challengeId; }

        public String getChallengeName() { return this.challengeName; }

        public String getChallengeDescription() { return this.challengeDescription; }

        public int getChallengeDuration() { return this.challengeDuration; }

        public int getChallengeLevel() { return this.challengeLevel; }

        public TState.STATE getChallengeState() { return this.challengeState; }
    }
}
