using System;

namespace BibleVerse.Events
{

    public class Event
    {
        //Base for event. Event must be logged to db on creation.
        private readonly BibleVerse.DALV2.BVIdentityContext _context;

        protected int _eventCode;
        protected string _eventType;
        protected string _eventContext;
        protected string _author;
        protected string _location;
        protected DTO.Event _event;

        public Event(BibleVerse.DALV2.BVIdentityContext context)
        {
            this._context = context;
            Initialize(string.Empty, string.Empty, string.Empty, string.Empty, 0);
            LogEvent();
        }

        public Event(BibleVerse.DALV2.BVIdentityContext context, string eventContext, string eventType)
        {
            this._context = context;
            Initialize(eventContext,eventType, string.Empty, string.Empty, 0);
            LogEvent();
        }

        public Event(BibleVerse.DALV2.BVIdentityContext context, string eventContext, string eventType, int eventCode)
        {
            this._context = context;
            Initialize(eventContext , eventType, string.Empty, string.Empty, eventCode);
            LogEvent();
        }

        public Event(BibleVerse.DALV2.BVIdentityContext context, string eventContext, string eventType, int eventCode, string author, string location)
        {
            this._context = context;
            Initialize(eventContext, eventType, author, location, eventCode);
            LogEvent();
        }

        private void Initialize(string eventContext, string eventType, string author, string location, int eventCode)
        {
            _eventCode = eventCode;
            _eventType = eventType;
            _eventContext = eventContext;
            _author = author;
            _location = location;
        }

        public DTO.Event LoggedEvent
        {
            get { return _event; }
        }

        private void LogEvent()
        {
            
            try
            {
                BibleVerse.DTO.Event newEvent = new DTO.Event();
                
                if (!String.IsNullOrEmpty(_eventType))
                {
                    newEvent.Name = _eventType;
                }
                else
                {
                    newEvent.Name = "NEW EVENT";
                }

                if (!String.IsNullOrEmpty(_eventContext))
                {
                    newEvent.Message = String.Format("Event Code: {0}, Event Type: {1}, Event Message: {2} ", _eventCode, _eventType, _eventContext);
                }
                else if (String.IsNullOrEmpty(_eventContext) && _eventCode > 0)
                {
                    newEvent.Message = String.Format("Event Code: {0}, Event Type: {1}, Event Message: {2} ", _eventCode, _eventType, "No Message Provided");
                }

                newEvent.Author = !String.IsNullOrEmpty(_author) ? _author : "Author Not Provided";
                newEvent.Location = !String.IsNullOrEmpty(_location) ? _location : "Location Not Provided";
                _event = newEvent;
                /*
                if (log.Severity == -1)
                {
                    Exception eventCodeDoesNotExist = new Exception()
                    {
                        Source = String.Format("eventor Code Does Not Exist! Attempted Log Code: {0}", _eventCode),
                        HResult = _invalideventCode
                    };
                    throw eventCodeDoesNotExist;
                }

                _log = log;
                */

                bool result = BVEventHelper.LogEvent(_event, _context);

                if (!result)
                {
                    //Create log err exception
                    Exception exceptionLogErr = new Exception()
                    {
                        Source = "Error during Event Log creation. Manual save in Event Log"
                    };

                    throw exceptionLogErr;
                }


            }
                
            catch (Exception ex)
            {
                /*
                if (ex.HResult == _invalideventCode)
                {
                    _log.Message += "\n" + ex.Source;
                    _log.Severity = _invalideventCode;
                }
                */
                BibleVerse.Exceptions.BVException exception = new Exceptions.BVException(_context, ex.Message, "BibleVerse.Events", 777);
            }
            
        }
    }
}
