using MultiTenancyFramework.Entities;
using MultiTenancyFramework.NHibernate.Audit;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiTenancyFramework.NHibernate.NHManager.Listeners
{
    /// <summary>
    /// This runs just before transaction is committed to the DB
    /// </summary>
    internal class AuditLogEventListener : AuditLogEventListener<long>
    {

    }

    /// <summary>
    /// This runs just before transaction is committed to the DB 
    /// </summary>
    /// <typeparam name="idT">The Id Type. I usually use long (Int64)</typeparam>
    internal class AuditLogEventListener<idT> : DefaultFlushEventListener, IMergeEventListener, IPreDeleteEventListener where idT : IEquatable<idT>
    {
        private HashSet<AuditLog> _auditLogItems { get; set; } = new HashSet<AuditLog>();

        public override void OnFlush(FlushEvent @event)
        {
            SaveAuditLogs(@event.Session);
            base.OnFlush(@event);
        }

        public bool OnPreDelete(PreDeleteEvent @event)
        {
            var entity = @event.Entity as IEntity<idT>;
            if (entity != null)
            {
                var sessionImpl = @event.Session.GetSessionImplementation();
                EntityEntry entry = sessionImpl.PersistenceContext.GetEntry(@event.Entity);
                entry.Status = Status.Loaded;
                entity.IsDeleted = true;
                if (entity.InstitutionCode == null || entity.InstitutionCode == Utilities.INST_DEFAULT_CODE) entity.InstitutionCode = string.Empty;

                object id = @event.Persister.GetIdentifier(@event.Entity, @event.Session.EntityMode);
                object[] fields = @event.Persister.GetPropertyValues(@event.Entity, @event.Session.EntityMode);
                object version = @event.Persister.GetVersion(@event.Entity, @event.Session.EntityMode);

                @event.Persister.Update(id, fields, new int[1], false, fields, version, @event.Entity, null, sessionImpl);

                PackageAuditLogItem(EventType.SoftDeleted, @event.Persister, @event.Entity, fields, entry.DeletedState);
            }
            return true;
        }

        public void OnMerge(MergeEvent @event)
        {
            DoMerge(@event);
        }

        public void OnMerge(MergeEvent @event, IDictionary copiedAlready)
        {
            DoMerge(@event);
        }

        private void DoMerge(MergeEvent @event)
        {
            var entity = @event.Original;
            var entityEntry = @event.Session.PersistenceContext.GetEntry(entity);
            if (entityEntry == null) return;
            object[] currentState;
            var modified = IsEntityModified(entityEntry, entity, @event.Session, out currentState);
            if (modified)
            {
                PackageAuditLogItem(EventType.Modified, entityEntry.Persister, entity, currentState, entityEntry.LoadedState);
            }
        }

        private void PackageAuditLogItem(EventType eventType, IEntityPersister persister, object obj, object[] currentValues, object[] oldValues)
        {
            if (currentValues == null) return;

            var entity = obj as IEntity<idT>;
            if (entity == null) return;
            if (entity.SkipAudit) return;

            if (entity.InstitutionCode == null || entity.InstitutionCode == Utilities.INST_DEFAULT_CODE) entity.InstitutionCode = string.Empty;

            if (typeof(IDoNotNeedAudit).IsAssignableFrom(entity.GetType())) return;

            entity.LastDateModified = DateTime.Now.GetLocalTime();
            var audit = CreateLogRecord(entity.InstitutionCode, NHUtils.CurrentUser, entity, entity.LastDateModified, eventType);
            if (audit != null)
            {
                audit.AuditData = AuditLogSerializer.SerializeData(persister, currentValues, oldValues);
                _auditLogItems.Add(audit);
            }
        }


        /// <summary>
        /// This packages the auditlog entity for you. After calling this, you should then set the SerializedData property.
        /// Since it's DB implementation-dependent, it's not done here.
        /// </summary>
        /// <param name="instCode"></param>
        /// <param name="user"></param>
        /// <param name="ent"></param>
        /// <param name="changeTime"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private AuditLog CreateLogRecord(string instCode, AppUser user, IEntity<idT> ent, DateTime changeTime, EventType eventType)
        {
            var auditLog = new AuditLog
            {
                EventType = eventType,
                EventDate = changeTime,
                EntityId = ent.Id.ToString(),
                InstitutionCode = instCode,
            };
            auditLog.Entity = ent.GetType().Name.AsSplitPascalCasedString();

            if (user != null)
            {
                auditLog.UserId = user.Id;
                auditLog.UserName = user.UserName;
            }
            try
            {
                auditLog.ClientIpAddress = HttpContext.Current?.Request?.UserHostAddress;
                auditLog.ClientName = HttpContext.Current?.Request?.UserHostName;
            }
            catch (Exception)
            {
                auditLog.ClientName = "[Could not resolve Client Name]";
            }

            auditLog.ApplicationName = ConfigurationHelper.SectionItem<string>("ClientConfiguration", "ApplicationName");

            return auditLog;
        }
        
        private void SaveAuditLogs(IEventSource session)
        {
            if (_auditLogItems.Count == 0) return;

            foreach (var audit in _auditLogItems)
            {
                session.Save(audit);
            }
            _auditLogItems.Clear();
        }

        private bool IsEntityModified(EntityEntry entry, object entity, ISessionImplementor session, out object[] currentState)
        {
            currentState = null;
            if (entry.Status != Status.Loaded) return false;
            if (!entry.ExistsInDatabase) return false;
            if (entry.LoadedState == null) return false;

            if (!entry.RequiresDirtyCheck(entity)) return false;

            IEntityPersister persister = entry.Persister;

            var currState = currentState = persister.GetPropertyValues(entity, session.EntityMode);
            object[] loadedState = entry.LoadedState;

            return persister.EntityMetamodel.Properties
                .Where((property, i) =>
                {
                    return !LazyPropertyInitializer.UnfetchedProperty.Equals(currState[i])
                            && property.Type.IsDirty(loadedState[i], currState[i], session);
                })
                .Any();
        }

    }
}
