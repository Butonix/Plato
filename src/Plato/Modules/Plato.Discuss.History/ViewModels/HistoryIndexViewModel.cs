﻿using Plato.Entities.History.Models;

namespace Plato.Discuss.History.ViewModels
{
    public class HistoryIndexViewModel
    {

        public EntityHistory History { get; set; }

        public EntityHistory LatestHistory { get; set; }

        public string Html { get; set; }

    }
}
