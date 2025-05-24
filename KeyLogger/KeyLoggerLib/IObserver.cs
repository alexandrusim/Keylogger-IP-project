/**************************************************************************
 *                                                                        *
 *  File:        IObserver.cs                                             *
 *  Copyright:   (c) 2025, Adelina-Petronela Hritcu,                      *
 *                         Raluca-Elena Musteata,                         *
 *                         Simiuc Alexandru,                              *
 *                         Vasilca Rares-Mihai                            *
 *                                                                        *
 *  E-mail:      raluca-elena.musteata@student.tuiasi.ro                  *
 *               petronela-adelina.hritcu@student.tuiasi.ro               *
 *               alexandru.simiuc@student.tuiasi.ro                       *  
 *               rares-mihai.vasilca@student.tuiasi.ro                    *
 *  Description: Implementare keylogger                                   *
 *               This application implements a keylogger in C# using      *
 *               the Observer design pattern. The project has a modular   *
 *               architecture, making the codebase easy to extend and     *
 *               maintain, and includes unit testing capabilities to      *
 *               ensure component reliability. The application is         *
 *               structured to be easily integrated into other projects   *
 *               or adapted for various keyboard monitoring scenarios.    *                                                         *
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
    /// Interfata pentru observatorii care primesc notificari despre apasarile de taste
    /// </summary>
    public interface IKeyLoggerObserver
    {
        /// <summary>
        /// Metoda apelata atunci cand o tasta este apasata
        /// </summary>
        /// <param name="key">Tasta apasata</param>
        void OnKeyPressed(string key);
    }

    /// <summary>
    /// Interfata pentru subiectul keylogger care gestioneaza atasarea, detasarea si notificarea observatorilor
    /// </summary>
    public interface IKeyLoggerSubject
    {
        /// <summary>
        /// Ataseaza un observator la subiect
        /// </summary>
        /// <param name="observer">Observatorul de atasat</param>
        void Attach(IKeyLoggerObserver observer);
        /// <summary>
        /// Detaseaza un observator de la subiect
        /// </summary>
        /// <param name="observer">Observatorul de detasat</param>
        void Detach(IKeyLoggerObserver observer);
        /// <summary>
        /// Notifica toti observatorii despre o tasta apasata
        /// </summary>
        /// <param name="key">Tasta apasata</param>
        void NotifyKeyPressed(string key);
    }
}
