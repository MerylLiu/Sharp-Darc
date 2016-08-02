#region << �� �� ע �� >>
/****************************************************
* �� �� ����
* Copyright(c) IT��ʦ
* CLR �汾: 4.0.30319.18408
* �� �� �ˣ�ITdos
* �������䣺admin@itdos.com
* �ٷ���վ��www.ITdos.com
* �������ڣ�2015/5/10 10:54:32
* �ļ�������
******************************************************
* �� �� �ˣ�ITdos
* �޸����ڣ�
* ��ע������
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Dos.ORM
{
    /// <summary>
    /// A delegate used for log.
    /// </summary>
    /// <param name="logMsg">The msg to write to log.</param>
    public delegate void LogHandler(string logMsg);

    /// <summary>
    /// Mark a implementing class as loggable.
    /// </summary>
    interface ILogable
    {
        /// <summary>
        /// OnLog event.
        /// </summary>
        event LogHandler OnLog;
    }
}
