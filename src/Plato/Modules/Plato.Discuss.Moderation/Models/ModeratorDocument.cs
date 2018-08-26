﻿using System.Collections.Generic;
using Plato.Internal.Abstractions;
using Plato.Internal.Models;

namespace Plato.Discuss.Moderation.Models
{
    public class ModeratorDocument : Serializable, IDocument
    {

        public int Id { get; set; }

        public IEnumerable<OldModerator> Moderators { get; set; }

    }
}
