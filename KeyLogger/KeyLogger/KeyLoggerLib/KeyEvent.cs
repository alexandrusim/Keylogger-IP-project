/**************************************************************************
 *                                                                        *
 *  File:        KeyEvent.cs                                              *
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
    /// clasa care tine tasta apasata si timpul cand s-a apasat
    /// </summary>
    public class KeyEvent
    {
        /// <summary>
        /// tasta apasata
        /// </summary>
        public string Key { get; set; } 

        /// <summary>
        /// timpul cand s-a apasat tasta
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now; 
    }
}
