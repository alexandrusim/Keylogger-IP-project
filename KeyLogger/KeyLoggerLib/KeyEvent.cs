/**************************************************************************
 *                                                                        *
 *  File:        KeyEvent.cs                                              *
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
 *               or adapted for various keyboard monitoring scenarios.    * 
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
    /// Clasa care reprezinta un eveniment de tastare - continand tasta apasata si momentul in care a avut loc
    /// </summary>
    public class KeyEvent
    {
        /// <summary>
        /// Tasta care a fost apasata
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Momentul in care tasta a fost apasata
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
