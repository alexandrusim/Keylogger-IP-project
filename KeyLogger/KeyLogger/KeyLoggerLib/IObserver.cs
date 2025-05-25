/**************************************************************************
 *                                                                        *
 *  File:        IObserver.cs                                             *
 *  Copyright:   (c) 2025, Adelina-Petronela Hritcu,                      *
 *                         Raluca-Elena Musteata,                         *
 *                         Alexandru Simiuc,                              *
 *                         Rares-Mihai Vasilca                            *
 *                                                                        *
 *  E-mail:      raluca-elena.musteata@student.tuiasi.ro                  *
 *               petronela-adelina.hritcu@student.tuiasi.ro               *
 *               alexandru.simiuc@student.tuiasi.ro                       *  
 *               rares-mihai.vasilca@student.tuiasi.ro                    *
 *  Description: implementare keylogger                                   *
 *               aplicatie care logheaza tastele apasate folosind         *
 *               modelul observer. structura modulara si usor de extins.  *
 *                                                                        *
 **************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keylogger
{
    /// <summary>
    /// interfata pentru observerii care primesc tasta apasata
    /// </summary>
    public interface IKeyLoggerObserver
    {
        /// <summary>
        /// se apeleaza cand se apasa o tasta
        /// </summary>
        /// <param name="key">tasta apasata</param>
        void OnKeyPressed(string key); 
    }

    /// <summary>
    /// interfata pentru subiectul care tine lista de observatori si trimite notificari
    /// </summary>
    public interface IKeyLoggerSubject
    {
        /// <summary>
        /// adauga un observer in lista
        /// </summary>
        /// <param name="observer">observer de adaugat</param>
        void Attach(IKeyLoggerObserver observer); 

        /// <summary>
        /// sterge un observer din lista
        /// </summary>
        /// <param name="observer">observer de sters</param>
        void Detach(IKeyLoggerObserver observer); 

        /// <summary>
        /// trimite la toti observerii tasta apasata
        /// </summary>
        /// <param name="key">tasta apasata</param>
        void NotifyKeyPressed(string key); 
    }
}
