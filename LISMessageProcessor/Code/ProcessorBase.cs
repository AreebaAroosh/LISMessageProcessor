using System;
using System.Collections.Generic;
using System.Text;

namespace LISMessageProcessor
{
    public abstract class ProcessorBase<T> 
    {
        public IMessageDecoder<T> _messageDecoder;

        public ProcessorBase(IMessageDecoder<T> messageDecoder)
        {
            _messageDecoder = messageDecoder;
        }
    }
}
