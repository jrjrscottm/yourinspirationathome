﻿//-----------------------------------------------------------------------
// <copyright file="JournalDbEngine.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2016 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2016 Akka.NET project <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Akka.Persistence.Sql.Common.Journal
{
    /// <summary>
    /// Persisted event identifier returning set of keys used to map particular instance of an event to database row id.
    /// </summary>
    public struct EventId
    {
        /// <summary>
        /// Database row identifier.
        /// </summary>
        public readonly long Id;

        /// <summary>
        /// Persistent event sequence number, monotonically increasing in scope of the same <see cref="PersistenceId"/>.
        /// </summary>
        public readonly long SequenceNr;

        /// <summary>
        /// Id of persistent actor, used to recognize all events related to that actor.
        /// </summary>
        public readonly string PersistenceId;

        public EventId(long id, long sequenceNr, string persistenceId)
        {
            Id = id;
            SequenceNr = sequenceNr;
            PersistenceId = persistenceId;
        }
    }

    /// <summary>
    /// Class used for storing whole intermediate set of write changes to be applied within single SQL transaction.
    /// </summary>
    public sealed class WriteJournalBatch
    {
        public readonly IDictionary<IPersistentRepresentation, IImmutableSet<string>> EntryTags;

        public WriteJournalBatch(IDictionary<IPersistentRepresentation, IImmutableSet<string>> entryTags)
        {
            EntryTags = entryTags;
        }
    }

    /// <summary>
    /// Message type containing set of all <see cref="Eventsourced.PersistenceId"/> received from the database.
    /// </summary>
    public sealed class AllPersistenceIds
    {
        public readonly ImmutableArray<string> Ids;

        public AllPersistenceIds(ImmutableArray<string> ids)
        {
            Ids = ids;
        }
    }
}