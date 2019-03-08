﻿using System.Collections.Generic;
using Plato.Entities.Labels.Models;

namespace Plato.Discuss.Labels.ViewModels
{
    public class LabelsViewModel
    {

        public int SelectedLabelId { get; set; }

        public IEnumerable<LabelBase> Labels { get; set; }
    }
}
