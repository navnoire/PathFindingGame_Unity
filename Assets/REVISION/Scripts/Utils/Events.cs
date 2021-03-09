using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NavnoireCoding.Utils.Enums;

namespace NavnoireCoding.Utils
{
    public class Events : Singleton<Events>
    {
        #region Game Management events
        /// <summary>
        /// prevState, incomingState
        /// </summary>
        public Action<GameState, GameState> OnGameStateChanged;
        #endregion

        #region UI events
        /// <summary>
        /// bool isFadeOut
        /// </summary>
        public Action<bool> OnMainMenuFadeComplete;
        #endregion

        #region Item events
        public Action<int> OnItemCollected;
        #endregion
    }
}

