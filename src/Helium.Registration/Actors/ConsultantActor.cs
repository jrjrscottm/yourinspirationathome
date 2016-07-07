using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;

namespace Helium.Registration.Actors
{
    public class ConsultantActor : ReceivePersistentActor
    {
        public static Props CreateProps(int consultantId)
        {
            return Props.Create(() => new ConsultantActor(consultantId));
        }

        #region Messages
        public class RegisterConsultant
        {
            public int ConsultantId { get; }

            public RegisterConsultant(int consultantId)
            {
                ConsultantId = consultantId;
            }
        }

        #endregion
        public override string PersistenceId { get; }

        private ConsultantState State { get; set; }

        public ConsultantActor(int consultantId)
        {
            PersistenceId = consultantId.ToString();

            Command<RegisterConsultant>(m =>
            {
                Persist(m, message =>
                {
                    State = new ConsultantState(message.ConsultantId);
                    //SaveSnapshot(State);
                });
            });

            Recover<SnapshotOffer>(
                offer => offer.Snapshot is ConsultantState,
                offer => State = offer.Snapshot as ConsultantState);

            Command<SaveSnapshotSuccess>(success => {
                DeleteMessages(success.Metadata.SequenceNr);
            });

            SetReceiveTimeout(TimeSpan.FromMinutes(5));
        }

        protected override bool Receive(object message)
        {
            if (message is ReceiveTimeout)
            {
                Self.GracefulStop(TimeSpan.FromSeconds(10));
            }

            return base.Receive(message);
        }

        protected override void OnRecoveryFailure(Exception reason, object message = null)
        {
            base.OnRecoveryFailure(reason, message);
            if (State == null)
            {
                Self.GracefulStop(TimeSpan.FromSeconds(3));
            }
            else
            {
                State = new ConsultantState(State.ConsultantId);
                DeleteSnapshots(new SnapshotSelectionCriteria(0));
                DeleteMessages(0);
                Persist(State, null);
            }
        }


        private class ConsultantState
        {
            public ConsultantState(int consultantId)
            {
                ConsultantId = consultantId;
                MyStuff = new List<string>();
            }

            public int ConsultantId { get; }
            public List<string> MyStuff { get; set; }

        }
    }
}
