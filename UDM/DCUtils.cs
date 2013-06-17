/* Filename:    DCUtils.cs
 * Program:     UDM
 * Version:     4.0
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: Set of utilities methods that are unique to Detailed 
 *              Characteristics.  Separated into a utilities class to reduce
 *              code bloat.
 * Includes Procedures
 *  dcCalcER()
 *  dcCalcGQ()
 *  dcGQTransactions()
 *  dcCalcHH()
 *  dcCalcHHP()
 *  dcControlER()
 *  dcControlGQ()
 *  dcControlHHP()
 *  dcControlMF()
 *  dcControlmh()
 *  dcControlSF()
 *  doDCRates()
 *  extractDCRates()
 *  mgraCalcER()
 *  mgraCalcGQ()
 *  mgraCalcHH()
 *  mgraCalcHHP()
 *  mgraCalcIncome()
 *  mgraCalcCiv()
 *  mgraCalcSectors()
 *  mgraControlCiv()
 *  mgraControlER()
 *  mgraControlGQ()
 *  mgraControlHH()
 *  mgraControlHHP()
 *  mgraERRates()
 *  mgraHHSRates()
 *  mgraVacRates()
 *  printEmp()
 *  printMInc()
 *  rebuildLUZBase()
 *  writeMB()
 *  writeZB()
 *  writeLUZHistory()
 *  updateLUZHistory()
 *  
 */

using System;
using System.IO;
using System.Windows.Forms;

namespace Sandag.TechSvcs.RegionalModels
{
  public class DCUtils
	{
        #region Constants
        private const bool MEMP_DEBUG = true;    
        private const bool mhH_DEBUG = true;    
        private const bool MINC_DEBUG = true;
       
        #endregion Constants


      /*****************************************************************************/

      /* method dcCalcER() */
      /// <summary>
      /// Method to perform ER calculations.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void dcCalcER( Detailed d, Master[] z,
                                     RegionalControls rc, StreamWriter dcOut,
                                     bool controlDCOvr )
        {
          int localSum = 0;
          int adjER = 0, rcAdjER = 0;
          double hhFac = 0;  
          int i;
          //-------------------------------------------------------------------------

            // ER
          dcOut.WriteLine( "ER before Controlling" );
          for(i = 0; i < d.NUM_LUZS; i++ )
          {
            z[i].fcst.p.er = ( int )( 0.5 + z[i].fcst.r.erHH * ( double )z[i].fcst.hh.total );
            if( controlDCOvr )      // If all LUZs are controlled
              localSum += z[i].fcst.p.er;     // Build local sum
            else      /* Otherwise only compute controlling sum from LUZs not overriden. */
            {
              if( !z[i].erOvr )
                localSum += z[i].fcst.p.er;
              else
                adjER += z[i].fcst.p.er;
            }  // end else
          }  // end for i

          // Compute adjusted regional total.  Adjustment may be zero.
          rcAdjER = rc.fcst.p.er - adjER;

          // Compute controlling factors
          if( localSum > 0 )
            hhFac = ( double )rcAdjER / ( double )localSum;

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            // If all LUZs controlled or this LUZ has no SF override
            if( controlDCOvr || ( !controlDCOvr && !z[i].erOvr ) )
              z[i].fcst.p.er = ( int )( 0.5 + ( double )z[i].fcst.p.er * hhFac );

            // Control ER to <= pop
            if( z[i].fcst.p.er > z[i].fcst.p.pop )
              z[i].fcst.p.er = z[i].fcst.p.pop;
          }  // end for i
          dcControlER( d, z, dcOut, rc, controlDCOvr );
          dcOut.WriteLine( "ER AFTER CONTROLLING" );
          for( i = 0; i < d.NUM_LUZS; i++ )
            dcOut.WriteLine( "{0,3} {1,6}", i + 1, z[i].fcst.p.er );
        }     // End method dcCalcER()

      /*****************************************************************************/
    
      /* method dcCalcGQ() */
      /// <summary>
      /// Method to perform GQ calculations.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void dcCalcGQ( Detailed d, Master[] z, RegionalControls rc )
        {
          int i;
          int [] fpop = new Int32[d.NUM_LUZS];
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
              // Compute the levels and increment totals
            z[i].fcst.p.gq = z[i].fcst.p.gqCiv + z[i].fcst.p.gqMil;
            z[i].fcst.pi.gq = z[i].fcst.p.gq - z[i].baseData.p.gq;
            z[i].fcst.pi.gqCiv = z[i].fcst.p.gqCiv - z[i].baseData.p.gqCiv;
            z[i].fcst.p.pop = z[i].fcst.p.gq + z[i].fcst.p.hhp;
            fpop[i] = z[i].fcst.p.pop;
            z[i].fcst.pi.pop = z[i].fcst.p.pop - z[i].baseData.p.pop;
            if( z[i].baseData.p.gq > 0 )
              z[i].fcst.pct.gq = 100 * ( double )z[i].fcst.pi.gq / ( double )z[i].baseData.p.gq;
            if( z[i].baseData.p.pop > 0 )
              z[i].fcst.pct.pop = 100 * ( double )z[i].fcst.pi.pop / ( double )z[i].baseData.p.pop;
          }   // end for i

        }     // End method dcCalcGQ()

        /*****************************************************************************/
    
      /* method dcCalcHH() */
      /// <summary>
      /// Method to perform HH calculations.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void dcCalcHH( Detailed d, Master[] z,
                                     StreamWriter dcOut, RegionalControls rc,
                                     bool controlDCOvr )
        {
          int localSumSF;
          int localSumMF;
          int localSummh;
          int i;

          int adjSF, rcAdjSF;
          int adjMF, rcAdjMF;
          int adjmh, rcAdjmh;
      
          double hhFacSF;
          double hhFacMF;
          double hhFacmh;
      
          dcOut.WriteLine( "DC_CALC PROCESSING - COMPUTING LUZ HH" );
          localSumSF = localSumMF = localSummh = 0;
          adjSF = adjMF = adjmh = 0;
          hhFacSF = hhFacMF = hhFacmh = 0.0;
          rcAdjSF = rcAdjMF = rcAdjmh = 0;

            // Build HS totals by structure type
          dcOut.WriteLine( "   BEFORE CONTROLLING" );
      
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            //int cc = 0;
            //if (i == 62)
            //   cc = 1;
              // Add increment to base for forecast
            z[i].fcst.hs.sf = z[i].baseData.hs.sf + z[i].fcst.hsi.sf;
            z[i].fcst.hs.mf = z[i].baseData.hs.mf + z[i].fcst.hsi.mf;
            z[i].fcst.hs.mh = z[i].baseData.hs.mh + z[i].fcst.hsi.mh;
        
              // Apply structure vacancy rates for hh
            if (z[i].fcst.hsi.sf == 0)
                z[i].fcst.hh.sf = z[i].baseData.hh.sf;
            else
                z[i].fcst.hh.sf = ( int )( 0.5 + z[i].fcst.hs.sf * ( 1.0 -( z[i].fcst.r.vSF / 100 ) ) );
            if (z[i].fcst.hsi.mf == 0)
                z[i].fcst.hh.mf = z[i].baseData.hh.mf;
            else
                z[i].fcst.hh.mf = ( int )( 0.5 + z[i].fcst.hs.mf * ( 1.0 -( z[i].fcst.r.vMF / 100 ) ) );

            if (z[i].fcst.hsi.mh == 0)
                z[i].fcst.hh.mh = z[i].baseData.hh.mh;
            else
                z[i].fcst.hh.mh = ( int )( 0.5 + z[i].fcst.hs.mh * ( 1.0 -( z[i].fcst.r.vmh / 100 ) ) );

            dcOut.WriteLine( "      LUZ {0,3} sf = {1,6} {2,6} mf = {3,6} {4,6} mh = {5,6} {6,6}", i + 1, z[i].fcst.hs.sf,
                             z[i].fcst.hh.sf, z[i].fcst.hs.mf, z[i].fcst.hh.mf, z[i].fcst.hs.mh, z[i].fcst.hh.mh);
            if( controlDCOvr )      // If all LUZs are controlled
            {
                // Build local sums
              localSumSF += z[i].fcst.hh.sf;
              localSumMF += z[i].fcst.hh.mf;
              localSummh += z[i].fcst.hh.mh;
            }  // end if

            else      /* Otherwise only compute controlling sum from LUZs not
                       * overriden */
            {
              if( !z[i].vacOvr )
              {
                localSumSF += z[i].fcst.hh.sf;
                localSumMF += z[i].fcst.hh.mf;
                localSummh += z[i].fcst.hh.mh;
              }  // end if
              else
              {
                adjSF += z[i].fcst.hh.sf;
                adjMF += z[i].fcst.hh.mf;
                adjmh += z[i].fcst.hh.mh;
              }   // end else
            }  // end else
          }     // End for i

            // Compute adjusted regional total - adjustment may be zero
          rcAdjSF = rc.fcst.hh.sf - adjSF;
          rcAdjMF = rc.fcst.hh.mf - adjMF;
          rcAdjmh = rc.fcst.hh.mh - adjmh;

            // Compute controlling factors
          if( localSumSF > 0 )
            hhFacSF = ( double )rcAdjSF / ( double )localSumSF;
          if( localSumMF > 0 )
            hhFacMF = ( double )rcAdjMF / ( double )localSumMF;
          if( localSummh > 0 )
            hhFacmh = ( double )rcAdjmh / ( double )localSummh;

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
              // If all luzs controlled or this luz has no sf override
            if( controlDCOvr || ( !controlDCOvr && !z[i].vacOvr ) )
            {
              z[i].fcst.hh.sf = ( int )( 0.5 + ( double )z[i].fcst.hh.sf * hhFacSF );
              z[i].fcst.hh.mf = ( int )( 0.5 + ( double )z[i].fcst.hh.mf * hhFacMF );
              z[i].fcst.hh.mh = ( int )( 0.5 + ( double )z[i].fcst.hh.mh * hhFacmh );
            }  // end if
              // Control HH to HS
            if( z[i].fcst.hh.sf > z[i].fcst.hs.sf )
              z[i].fcst.hh.sf = z[i].fcst.hs.sf;
            if( z[i].fcst.hh.mf > z[i].fcst.hs.mf )
              z[i].fcst.hh.mf = z[i].fcst.hs.mf;
            if( z[i].fcst.hh.mh > z[i].fcst.hs.mh )
              z[i].fcst.hh.mh = z[i].fcst.hs.mh;
          }  // end for i

          dcControlSF( d, z, rc, controlDCOvr );
          dcControlMF( d, z, rc, controlDCOvr );
          dcControlmh( d, z, rc, controlDCOvr );

          dcOut.WriteLine( "   AFTER CONTROLLING" );
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dcOut.WriteLine( "      LUZ {0,3} sf = {1,6} {2,6} mf = {3,6} {4,6} mh = {5,6} {6,6}", i + 1, z[i].fcst.hs.sf,
                             z[i].fcst.hh.sf, z[i].fcst.hs.mf, z[i].fcst.hh.mf, z[i].fcst.hs.mh, z[i].fcst.hh.mh );

              // Compute HH increment from forecast and base
            z[i].fcst.hhi.sf = z[i].fcst.hh.sf - z[i].baseData.hh.sf;
            z[i].fcst.hhi.mf = z[i].fcst.hh.mf - z[i].baseData.hh.mf;
            z[i].fcst.hhi.mh = z[i].fcst.hh.mh - z[i].baseData.hh.mh;
       
              // Do totals
            z[i].fcst.hhi.total = 0;
            z[i].fcst.hh.total = 0;
      
              // Increment
            z[i].fcst.hhi.total = z[i].fcst.hhi.sf + z[i].fcst.hhi.mf + z[i].fcst.hhi.mh;
            z[i].fcst.hh.total = z[i].fcst.hh.sf + z[i].fcst.hh.mf + z[i].fcst.hh.mh;     // Levels

              // Pct change
            if( z[i].baseData.hh.total > 0 )
              z[i].fcst.pct.hh = ( double )z[i].fcst.hhi.total / ( double)z[i].baseData.hh.total * 100;
          }     // End for i
        }     // End method dcCalcHH()

      /*****************************************************************************/

      /* method dcCalcHHP() */
      /// <summary>
      /// Method to perform HHP calculations.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void dcCalcHHP( Detailed d, Master[] z, RegionalControls rc, bool controlDCOvr )
        {
          int localSumHHP = 0;
          int i;
          int adjHHP = 0, rcAdjHHP = 0;
          double hhFacHHP = 0;

            // HHP
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            z[i].fcst.p.hhp = ( int )( 0.5 + z[i].fcst.r.hhs * ( double )z[i].fcst.hh.total );
            if( controlDCOvr )      // If all LUZs are controlled
              localSumHHP += z[i].fcst.p.hhp;     // Build local sum

            else      /* Otherwise only compute controlling sum from LUZs not overriden */
            {
              if( !z[i].hhsOvr )
                localSumHHP += z[i].fcst.p.hhp;
              else
                adjHHP += z[i].fcst.p.hhp;
            }  // end else
          }   // end for i

            // Compute adjusted regional total - adjustment may be zero
          rcAdjHHP = rc.fcst.p.hhp - adjHHP;

            // Compute controlling factors
          if( localSumHHP > 0 )
            hhFacHHP = ( double )rcAdjHHP / ( double )localSumHHP;

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
              // If all LUZs controlled or this LUZ has no SF override
            if( controlDCOvr || ( !controlDCOvr && !z[i].hhsOvr ) )
              z[i].fcst.p.hhp = ( int )( 0.5 + ( double )z[i].fcst.p.hhp * hhFacHHP );
              // Control HHP to >= HH
            if( z[i].fcst.p.hhp < z[i].fcst.hh.total )
              z[i].fcst.p.hhp = z[i].fcst.hh.total;
          }
          dcControlHHP( d, z, rc, controlDCOvr );
        }     // End procedure dcCalcHHP()

      /*****************************************************************************/

      /* method dcControlER() */
      /// <summary>
      /// Method to control the ER estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlER( Detailed d, Master[] z,
                                         StreamWriter dcOut, RegionalControls rc,
                                         bool controlDCOvr )
        {
          int realIndex;
          int ret;
          int ter = 0;
          int i;

          bool[] oFlag = new bool[d.NUM_LUZS];
          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          int[] ubound = new int[d.NUM_LUZS];

          for(i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.p.er;
            ter += z[i].fcst.p.er;
            dcOut.WriteLine( "{0,3} {1,6}", i + 1, z[i].fcst.p.er );
          }
          dcOut.WriteLine( "Tot {0,6}", ter );
      
          if( ter != rc.fcst.p.er )
          {
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS - 1 );      // Sort them
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
              ubound[i] = z[realIndex].fcst.p.pop;      // Load cap with population

              if( !controlDCOvr )
                oFlag[i] = z[realIndex].erOvr;
              else
                oFlag[i] = false;
            }
            ret = UDMUtils.roundItUpperLimit( sortedData, oFlag, ubound,rc.fcst.p.er, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "controlER roundItUpperLimit did not converge - difference = " + ret );

              // Restore the rounded totals
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.p.er = sortedData[i];
              z[realIndex].fcst.pi.er = z[realIndex].fcst.p.er - z[realIndex].baseData.p.er;
            }
          }     // End if
        }     // End method dcControlER()

      /*****************************************************************************/

      /* method dcControlGQ() */
      /// <summary>
      /// Method to control the GQ estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlGQ( Detailed d, Master[] z, RegionalControls rc )
        {
          int realIndex;
          int ret;
          int hht = 0;
          int target;
          int i;

          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.p.gqCivExtra;
            hht += dIndex1[i,1];
          }
   
          target = rc.fcst.pi.gqCiv - rc.site.gqCiv;
          if( hht != target )
          {
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS - 1 );      // Sort them
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
            }
            ret = UDMUtils.roundItNoLimit( sortedData, target, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "dcCalcGQ roundItNoLimit did not converge - difference = " + ret );
              // Restore the rounded totals
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.pi.gqCiv = sortedData[i] + z[realIndex].site.gqCiv;
            }
          }     // End if
        }     // End method dcControlGQ()

      /*****************************************************************************/

      /* method dcControlHHP() */
      /// <summary>
      /// Method to control the HHP estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlHHP( Detailed d, Master[] z, RegionalControls rc, bool controlDCOvr )
        {
          int realIndex;
          int ret;
          int hht = 0;
          int i;

          bool[] oFlag = new bool[d.NUM_LUZS];
          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          int[] lbound = new int[d.NUM_LUZS];

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.p.hhp;
            hht += z[i].fcst.p.hhp;
          }
          if( hht != rc.fcst.p.hhp )
          {
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS - 1 );     // Sort them
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
              lbound[i] = z[realIndex].fcst.hh.total;
              if( !controlDCOvr )
                oFlag[i] = z[realIndex].hhsOvr;
              else
                oFlag[i] = false;
            }
            ret = UDMUtils.roundItLowerLimit( sortedData, oFlag, lbound, rc.fcst.p.hhp, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "HHP dcCalc roundItLowerLimit did not converge - difference = " + ret );
              // Restore the rounded totals
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.p.hhp = sortedData[i];
              z[realIndex].fcst.pi.hhp = z[realIndex].fcst.p.hhp - z[realIndex].baseData.p.hhp;
              if( z[realIndex].baseData.p.hhp > 0 )
                z[realIndex].fcst.pct.hhp = 100 * ( double )z[realIndex].fcst.pi.hhp / ( double )z[realIndex].baseData.p.hhp;
            }
          }     // End if
        }     // End method dcControlHHP()

      /*****************************************************************************/

      /* method dcControlMF() */
      /// <summary>
      /// Method to control the MF HH estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlMF( Detailed d, Master[] z, RegionalControls rc, bool controlDCOvr )
        {
          int realIndex;
          int ret;
          int hht = 0;
          int i;
      
          bool[] oFlag = new bool[d.NUM_LUZS];
          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          int[] ubound = new int[d.NUM_LUZS];

            // MF
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.hh.mf;
            hht += z[i].fcst.hh.mf;
          }  // end for
      
          if( hht != rc.fcst.hh.mf )
          {
              /* sort them */
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS - 1 );
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
              ubound[i] = z[realIndex].fcst.hs.mf;
              if( !controlDCOvr )
                oFlag[i] = z[realIndex].vacOvr;
              else
                oFlag[i] = false;
            }  // end for
            ret = UDMUtils.roundItUpperLimit( sortedData, oFlag, ubound,rc.fcst.hh.mf, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "MF HH dcCalc rounditUpperLimit did not converge - difference = " + ret );

              /* restore the rounded totals */
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.hh.mf = sortedData[i];
            }  // end for
          }     // End if
        }     // End method dcControlMF()

      /*****************************************************************************/

      /* method dcControlmh() */
      /// <summary>
      /// Method to control the mh HH estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlmh( Detailed d, Master[] z, RegionalControls rc, bool controlDCOvr )
        {
          int realIndex;
          int ret;
          int hht = 0;
          int i;
      
          bool[] oFlag = new bool[d.NUM_LUZS];
          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          int[] ubound = new int[d.NUM_LUZS];

            // mh
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.hh.mh;
            hht += z[i].fcst.hh.mh;
          } // end for
          
          if( hht != rc.fcst.hh.mh )
          {
              // Sort them
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS - 1 );
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
              ubound[i] = z[realIndex].fcst.hs.mh;
              if( !controlDCOvr )
                oFlag[i] = z[realIndex].vacOvr;
              else
                oFlag[i] = false;
            }  // end for

            ret = UDMUtils.roundItUpperLimit( sortedData, oFlag, ubound,rc.fcst.hh.mh, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "controlmh rounditUpperLimit did not converge - difference = " + ret );

              // Restore the rounded totals
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.hh.mh = sortedData[i];
            }  // end for
          }     // End if
        }     // End method dcControlmh()

      /*****************************************************************************/

      /* method dcControlSF() */
      /// <summary>
      /// Method to control the SF HH estimates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        private static void dcControlSF( Detailed d, Master[] z,RegionalControls rc, bool controlDCOvr )
        {
          int realIndex;
          int ret;
          int hht = 0;
          int i;

          bool[] oFlag = new bool[d.NUM_LUZS];
          int[,] dIndex1 = new int[d.NUM_LUZS,2];
          int[] sortedData = new int[d.NUM_LUZS];
          int[] ubound = new int[d.NUM_LUZS];
      
            // Now use the + - roundoff to match the regional totals

            // SF
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            dIndex1[i,0] = i;
            dIndex1[i,1] = z[i].fcst.hh.sf;
            hht += z[i].fcst.hh.sf;
          }  // end for

          if( hht != rc.fcst.hh.sf )
          {
              // Sort them
            UDMUtils.quickSort( dIndex1, 0, d.NUM_LUZS-1 );
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              sortedData[i] = dIndex1[i,1];
              realIndex = dIndex1[i,0];
              ubound[i] = z[realIndex].fcst.hs.sf;
              if( !controlDCOvr )
                oFlag[i] = z[realIndex].vacOvr;
              else
                oFlag[i] = false;
            }  // end for
            ret = UDMUtils.roundItUpperLimit( sortedData, oFlag, ubound,rc.fcst.hh.sf, d.NUM_LUZS );
            if( ret > 0 )
              MessageBox.Show( "controlSF dcCalc rounditUpperLimit did not converge - difference = " + ret );
              // Restore the rounded totals
            for( i = 0; i < d.NUM_LUZS; i++ )
            {
              realIndex = dIndex1[i,0];
              z[realIndex].fcst.hh.sf = sortedData[i];
            }  // end for
          }     // End if
        }     // End method dcControlSF()

      /*****************************************************************************/

      /* method doDCRates() */
      /// <summary>
      /// Method to calculate DC rates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/08/97   tb    Initial coding
       *                 08/22/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void doDCRates( Detailed d, Master[] z, StreamWriter dcOut )
        {
          int i;

          //d.writeToStatusBox( "Computing LUZ rates.." );
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
              // Build HS and HH total
            z[i].fcst.hh.total = z[i].fcst.hh.sf + z[i].fcst.hh.mf + z[i].fcst.hh.mh;
            z[i].fcst.hs.total = z[i].fcst.hs.sf + z[i].fcst.hs.mf + z[i].fcst.hs.mh;
              // Vacancy rates
            if( z[i].fcst.hs.sf > 0 )
              z[i].fcst.r.vSF = ( 1.0 - ( ( double )z[i].fcst.hh.sf / ( double )z[i].fcst.hs.sf ) ) * 100;
            else
              z[i].fcst.r.vSF = 0;

            if( z[i].fcst.hs.mf > 0 )
              z[i].fcst.r.vMF = ( 1.0 - ( ( double )z[i].fcst.hh.mf / ( double )z[i].fcst.hs.mf ) ) * 100;
            else
              z[i].fcst.r.vMF = 0;

            if( z[i].fcst.hs.mh > 0 )
              z[i].fcst.r.vmh = ( 1.0 - ( ( double )z[i].fcst.hh.mh / ( double )z[i].fcst.hs.mh ) ) * 100;
            else
              z[i].fcst.r.vmh = 0;

            if( z[i].fcst.hs.total > 0 )
              z[i].fcst.r.v = ( 1.0 - ( ( double )z[i].fcst.hh.total / ( double )z[i].fcst.hs.total ) ) * 100;
            else
              z[i].fcst.r.v = 0;
        
            dcOut.WriteLine( "LUZ {0,3} Vacancy Rates {1,6:F2} {2,6:F2} {3,6:F2}{4,6:F2}", i + 1, z[i].fcst.r.vSF, z[i].fcst.r.vMF,z[i].fcst.r.vmh, z[i].fcst.r.v );
              // Employed residents/HH
            if( z[i].fcst.hh.total > 0 )
              z[i].fcst.r.erHH = ( double )z[i].fcst.p.er / ( double )z[i].fcst.hh.total;
            else
              z[i].fcst.r.erHH = 0;

              // HHS
            if( z[i].fcst.hh.total > 0 )
              z[i].fcst.r.hhs = ( double )z[i].fcst.p.hhp / ( double )z[i].fcst.hh.total;
            else
              z[i].fcst.r.hhs = 0;

              // Rates for increment

              // Vacancy rates
            z[i].fcst.ri.vSF = z[i].fcst.r.vSF - z[i].baseData.r.vSF;
            z[i].fcst.ri.vMF = z[i].fcst.r.vMF - z[i].baseData.r.vMF;
            z[i].fcst.ri.vmh = z[i].fcst.r.vmh - z[i].baseData.r.vmh;
            z[i].fcst.ri.v = z[i].fcst.r.v - z[i].baseData.r.v;
            z[i].fcst.ri.hhs = z[i].fcst.r.hhs - z[i].baseData.r.hhs;
            z[i].fcst.ri.erHH = z[i].fcst.r.erHH - z[i].baseData.r.erHH;
        
            if( z[i].baseData.r.v > 0 )
              z[i].fcst.pct.v = ( ( double )z[i].fcst.r.v / ( double )z[i].baseData.r.v - 1 ) * 100;
            else
              z[i].fcst.pct.v = 0;

            if( z[i].baseData.r.vSF > 0 )
              z[i].fcst.pct.vSF = ( ( double )z[i].fcst.r.vSF / ( double )z[i].baseData.r.vSF - 1 ) * 100;
            else
              z[i].fcst.pct.vSF = 0;

            if( z[i].baseData.r.vMF > 0 )
              z[i].fcst.pct.vMF = ( ( double )z[i].fcst.r.vMF / ( double )z[i].baseData.r.vMF - 1 )  * 100;
            else
              z[i].fcst.pct.vMF = 0;

            if( z[i].baseData.r.vmh > 0 )
              z[i].fcst.pct.vmh = ( ( double )z[i].fcst.r.vmh / ( double )z[i].baseData.r.vmh - 1 ) * 100;
            else
              z[i].fcst.pct.vmh = 0;

            if( z[i].baseData.r.hhs > 0 )
              z[i].fcst.pct.hhs = ( ( double )z[i].fcst.r.hhs / ( double )z[i].baseData.r.hhs - 1 ) * 100;
            else
              z[i].fcst.pct.hhs = 0;

            if( z[i].baseData.r.erHH > 0 )
              z[i].fcst.pct.erHH = ( ( double )z[i].fcst.r.erHH / ( double )z[i].baseData.r.erHH - 1 ) * 100;
            else
              z[i].fcst.pct.erHH = 0;
          }     // End for i
        }     // End method doDCRates()

      /*****************************************************************************/

      /* method extractDCRates() */
      /// <summary>
      /// Method to restore LUZ rates from temp table produced by module 3 (HS)
      /// These come from ASCII file hsRatesPass.txt.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/01/97   tb    Initial coding
       *                 08/20/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static bool extractDCRates( BaseForm form, Master[] z, Master reg, StreamWriter dcOut )
        {
          int ii;
          StreamReader hsr = null;
          try
          {
            hsr = new StreamReader( form.networkPath + String.Format(form.appSettings["hsRatesPass"].Value));
          }
          catch( IOException i )
          {
            MessageBox.Show( "An error occurred while trying to open the file.  " +  i.ToString() );
            return false;
          }
      
          // Read in LUZ data
          //dcOut.WriteLine( "FCST LUZ VACANCY RATES" );

          string line;
          string[] tokens;
          int tokenCount;

          for( int i = 0; i < form.NUM_LUZS; i++ )
          {
            line = hsr.ReadLine();
              /* After reading in a line, parse it down to tokens.  C#'s weak 
               * string splitter/tokenizer is most disappointing, leaving array 
               * elements of empty strings mixed with valid tokens. */
            tokens = line.Split( ' ' );
            tokenCount = 0;
              /* Logically remove the empty elements by pushing only the valid 
               * tokens to the head of the array. */
            for( int j = 0; j < tokens.Length; j++ )
              if( tokens[j].Length > 0 )
                tokens[tokenCount++] = tokens[j];
              // Parse data out of the array of string tokens.
            ii = int.Parse( tokens[0] );
            z[i].baseData.r.vSF = double.Parse( tokens[1] );
            z[i].baseData.r.vMF = double.Parse( tokens[2] );
            z[i].baseData.r.vmh = double.Parse( tokens[3] );
            z[i].baseData.r.erHH = double.Parse( tokens[4] );
            z[i].baseData.r.hhs = double.Parse( tokens[5] );
            z[i].fcst.r.vSF = double.Parse( tokens[6] );
            z[i].fcst.r.vMF = double.Parse( tokens[7] );
            z[i].fcst.r.vmh = double.Parse( tokens[8] );
            z[i].fcst.r.erHH = double.Parse( tokens[9] );
            z[i].fcst.r.hhs = double.Parse( tokens[10] );
            dcOut.WriteLine( "LUZ {0,3} sf = {1,6:F2} mf = {2,6:F2} mh = {3,6:F2}",(i + 1), z[i].fcst.r.vSF, z[i].fcst.r.vMF,z[i].fcst.r.vmh );
          }
      
            // Now get regional rates
          line = hsr.ReadLine();
          tokens = line.Split( new char[] {' '} );
          tokenCount = 0;
          for (int j = 0; j < tokens.Length; j++)
          {
              if (tokens[j].Length > 0)
                  tokens[tokenCount++] = tokens[j];
          }  // end for
          reg.baseData.r.vSF = double.Parse( tokens[0] );
          reg.baseData.r.vMF = double.Parse( tokens[1] );
          reg.baseData.r.vmh = double.Parse( tokens[2] );
          reg.baseData.r.erHH = double.Parse( tokens[3] );
          reg.baseData.r.hhs = double.Parse( tokens[4] );
          reg.fcst.r.vSF = double.Parse( tokens[5] );
          reg.fcst.r.vMF = double.Parse( tokens[6] );
          reg.fcst.r.vmh = double.Parse( tokens[7] );
          reg.fcst.r.erHH = double.Parse( tokens[8] );
          reg.fcst.r.hhs = double.Parse( tokens[9] );
          dcOut.WriteLine( " REGION sf = {0,6:F2} mf = {1,6:F2} mh = {2,6:F2}", reg.fcst.r.vSF, reg.fcst.r.vMF, reg.fcst.r.vmh );
          return true;
        }     // End method extractDCRates()

      /*****************************************************************************/

      /* method mgraCalcER() */
      /// <summary>
      /// Method to calculate MGRA ER.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/11/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcER( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int j;

          double[] t25ER = new double[d.NUM_LUZS];
          d.writeToStatusBox( "Computing MGRA ER.." );
          for( int i = 0; i < d.NUM_LUZS; i++ )
          {
              // Store % change in LUZ vacancy rates
            if( z[i].baseData.r.erHH > 0 )
              t25ER[i] = z[i].fcst.r.erHH / z[i].baseData.r.erHH;
            else
              t25ER[i] = 1.0;
          }  // end for
          for( j = 0; j < d.NUM_MGRAS; j++ )
          {
              // Derive MGRA ER rates
            mgraERRates( j, t25ER, mbData, z );
              // Compute ER fcst - apply ER/HH to HH
            mbData[j].fcst.p.er = ( int )( 0.5 + ( double )mbData[j].fcst.hh.total * mbData[j].fcst.r.erHH );
            if( mbData[j].fcst.p.er > mbData[j].fcst.p.pop )
              mbData[j].fcst.p.er = mbData[j].fcst.p.pop;
          }  // end for
            // Control the MGRA HH to LUZ totals
          mgraControlER( d, mbData, z );
        }     // End method mgraCalcER()

      /*****************************************************************************/

      /* method mgraCalcHH() */
      /// <summary>
      /// Method to calculate MGRA HH.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/11/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcHH( Detailed d, MBMaster[] mbData, Master[] z )
        {
          double[] t23SF = new double[d.NUM_LUZS];
          double[] t23MF = new double[d.NUM_LUZS];
          double[] t23mh = new double[d.NUM_LUZS];
          d.writeToStatusBox( "Computing MGRA HH.." );
          for( int i = 0; i < d.NUM_LUZS; i++ )
          {
              // Store % change in LUZ vacancy rates
            if (z[i].baseData.r.vSF > 0)
              t23SF[i] = z[i].fcst.r.vSF / z[i].baseData.r.vSF;
            else
              t23SF[i] = 0;
            if (z[i].baseData.r.vMF > 0)
              t23MF[i] = z[i].fcst.r.vMF / z[i].baseData.r.vMF;
            else
              t23MF[i] = 0;
            if (z[i].baseData.r.vmh > 0)
              t23mh[i] = z[i].fcst.r.vmh / z[i].baseData.r.vmh;
            else
              t23mh[i] = 0;
          }  // end for

          for( int j = 0; j < d.NUM_MGRAS; j++ )
          {
            // Compute MGRA HS levels
            
            mbData[j].fcst.hs.sf = mbData[j].baseData.hs.sf + mbData[j].fcst.hsi.sf;
            mbData[j].fcst.hs.mf = mbData[j].baseData.hs.mf + mbData[j].fcst.hsi.mf;
            
            mbData[j].fcst.hs.mh = mbData[j].baseData.hs.mh + mbData[j].fcst.hsi.mh;
            if( mbData[j].fcst.hs.sf < 0 || mbData[j].fcst.hs.mf < 0 || mbData[j].fcst.hs.mh < 0 )
              MessageBox.Show( "     Negative stock" );
        
              // Derive MGRA vacancy rates
            mgraVacRates( j, t23SF, t23MF, t23mh, mbData, z );

              // Compute HH fcst - apply vac rates to HS
            if (mbData[j].fcst.hsi.sf == 0)
                mbData[j].fcst.hh.sf = mbData[j].baseData.hh.sf;
            else
                mbData[j].fcst.hh.sf = ( int )( 0.5 + ( double )mbData[j].fcst.hs.sf * ( 1.0 - mbData[j].fcst.r.vSF / 100 ) );
            if (mbData[j].fcst.hsi.mf == 0)
                mbData[j].fcst.hh.mf = mbData[j].baseData.hh.mf;
            else
                mbData[j].fcst.hh.mf = ( int )( 0.5 + ( double )mbData[j].fcst.hs.mf * ( 1.0 - mbData[j].fcst.r.vMF / 100 ) );
            if (mbData[j].fcst.hsi.mh == 0)
                mbData[j].fcst.hh.mh = mbData[j].baseData.hh.mh;
            else
                mbData[j].fcst.hh.mh = ( int )( 0.5 + ( double )mbData[j].fcst.hs.mh * ( 1.0 - mbData[j].fcst.r.vmh / 100 ) );
          }     // End for j

            // Control the MGRA HH to LUZ totals
          mgraControlHH( d, mbData, z );

            // Compute some totals for use by other routines
          for( int j = 0; j < d.NUM_MGRAS; j++ )
          {
              // Total housing stock
            mbData[j].fcst.hs.total = mbData[j].fcst.hs.sf + mbData[j].fcst.hs.mf + mbData[j].fcst.hs.mh;
              // Total housing stock increment
            mbData[j].fcst.hsi.total = mbData[j].fcst.hsi.sf + mbData[j].fcst.hsi.mf + mbData[j].fcst.hsi.mh;
              // Households increment
            mbData[j].fcst.hhi.sf = mbData[j].fcst.hh.sf - mbData[j].baseData.hh.sf;
            mbData[j].fcst.hhi.mf = mbData[j].fcst.hh.mf - mbData[j].baseData.hh.mf;
            mbData[j].fcst.hhi.mh = mbData[j].fcst.hh.mh - mbData[j].baseData.hh.mh;
              // Total households
            mbData[j].fcst.hh.total = mbData[j].fcst.hh.sf + mbData[j].fcst.hh.mf + mbData[j].fcst.hh.mh;
              // Households total increment
            mbData[j].fcst.hhi.total = mbData[j].fcst.hh.total - mbData[j].baseData.hh.total;
            int t = 0;
            if (mbData[j].fcst.hhi.total < 0)
                t = 1;
          }     // End for j
        }     // End method mgraCalcHH()

      /*****************************************************************************/

      /* method mgraCalcHHP() */
      /// <summary>
      /// Method to calculate MGRA HHP.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/15/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcHHP( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int i,j;
          double[] t24HHP = new double[d.NUM_LUZS];
          d.writeToStatusBox( "Computing MGRA HHP" );
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
              // Store % change in LUZ vacancy rates
            if( z[i].baseData.r.hhs > 0 )
              t24HHP[i] = z[i].fcst.r.hhs / z[i].baseData.r.hhs;
            else
              t24HHP[i] = 1.0;
          }  // emd for
          for( j = 0; j < d.NUM_MGRAS; j++ )
          {
              // Derive MGRA HHS
            mgraHHSRates( mbData, z, j, t24HHP );

              // Compute HH fcst - apply vac rates to hs
            mbData[j].fcst.p.hhp = ( int )( 0.5 + ( double )mbData[j].fcst.hh.total * mbData[j].fcst.r.hhs );
              // Constrain HHP to at least HH
            if( mbData[j].fcst.p.hhp < mbData[j].fcst.hh.total )
              mbData[j].fcst.p.hhp = mbData[j].fcst.hh.total;
          }  // end for

            // Control the MGRA HH to LUZ totals
          mgraControlHHP( d, mbData, z );
        }     // End method mgraCalcHHP()

      /*****************************************************************************/

      /* method mgraCalcIncome() */
      /// <summary>
      /// Method to compute and control MGRA income distribution.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/16/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcIncome( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int i, j, k;
          int mgraId;
          int ret;
          int tSum;
          int nmgras;
          int[] colTotal = new int[d.NUM_HH_INCOME];
          int[] rowTotal = new int[d.MAX_MGRAS_IN_LUZ];
          int[] mgraHHI = new int[d.MAX_MGRAS_IN_LUZ];      // Row marginal
          int[] luzHHI = new int[d.NUM_HH_INCOME];
          int[] ptc = new int[d.MAX_MGRAS_IN_LUZ];
          int[] ptr = new int[d.NUM_HH_INCOME];
          int[] ptmgraId = new int[d.MAX_MGRAS_IN_LUZ];
          int[,] tempMGRA = new int[d.MAX_MGRAS_IN_LUZ,d.NUM_HH_INCOME];

           StreamWriter mInc = null;

          d.writeToStatusBox( "Computing MGRA income.." );
          if( MINC_DEBUG )
          {
            try
            {
              mInc = new StreamWriter( new FileStream( string.Format(d.appSettings["mgraInc"].Value),FileMode.Create ) );
              mInc.AutoFlush = true;
            }
            catch( IOException e )
            {
              MessageBox.Show( e.ToString(), e.GetType().ToString() );

            }
          }  // end if

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
        
            if( MINC_DEBUG )
              d.writeToStatusBox( "   processing LUZ " + ( i + 1 ) );
            nmgras = 0;
          
              /* Fill the temp array with MGRA income data.  This simplifies passing data to pachinko. */
            for( k = 0; k < d.NUM_MGRAS; k++ )
            {
                // Find the LUZ for this MGRA
              if( mbData[k].luz - 1 == i )
              {
                  // Store the MGRA IDs
                ptmgraId[nmgras] = k;
                for( j = 0; j < d.NUM_HH_INCOME; j++ )
                {
                  // Move the income distribution data to temp array
                  tempMGRA[nmgras,j] = mbData[k].baseData.i.hh[j];

                  // Get LUZ col totals as sum across MGRAs from base year data
                  colTotal[j] += mbData[k].baseData.i.hh[j];
                }  // end for
                nmgras++;     // Increment the MGRA counter for this LUZ
              }     // End if
            }     // End for k

            printMInc( d, mInc, i, tempMGRA, ptmgraId, colTotal, nmgras );

              /* tempMGRA now stores the base income distribution for the MGRAs in this LUZ.  colTotal stores base income dist totals.  Do any
               * negative LUZ column totals - see doc on LUZ decrements.  Pachinko arguments are LUZ column marginals and MGRA HH marginals. */
        
              // Load row and col marginals
            for( k = 0; k < nmgras; k++ )
            {
              mgraId = ptmgraId[k];
              mgraHHI[k] = mbData[mgraId].fcst.hhi.total;     /* MGRA HH unit change */
            }  // end for

              // Load LUZ inc distribution for col marginals.
            for( j = 0; j < d.NUM_HH_INCOME; j++ )
              luzHHI[j] = z[i].fcst.ii.hh[j];

              // Start controlling here
            for( j = 0; j < d.NUM_HH_INCOME; j++ )
            {
              for( k = 0; k < nmgras; k++ )
                ptc[k] = tempMGRA[k,j];

                // Look for LUZ income category decrements
              if( z[i].fcst.ii.hh[j] < 0 )
              {
                // Call special pachinko 1 for neg LUZ changes
           
                ret = UDMUtils.specialPachinko1( ptc, mgraHHI, ref luzHHI[j], nmgras );
                if( ret > 0 && ret != 99999 )
                {
                  MessageBox.Show( "sp1 failed LUZ " + ( i + 1 ) + " income " + ( j + 1 ) + " return = " + ret, "Error" );
                }  // end if
                else if( ret == 99999 )
                {
                  MessageBox.Show( "Failed to find sp1 pachinko distribution!", "Error" );
             
                }  // end else
                  // Restore
                for( k = 0; k < nmgras; k++ )
                  tempMGRA[k,j] = ptc[k];
              }  // end if

                // Compute col sums of MGRA data
              colTotal[j] = 0;
              for( k = 0; k < nmgras; k++ )
                colTotal[j] += tempMGRA[k,j];
            }     // End for j

            printMInc( d,mInc, i, tempMGRA, ptmgraId, colTotal, nmgras );

            //if( MINC_DEBUG )
              //d.writeToStatusBox( "      neg rows" );
          
            // Now do rows with neg MGRA change
            for( k = 0; k < nmgras; k++ )
            {
              mgraId = ptmgraId[k];
              if( mgraHHI[k] < 0 )
              {
                tSum = 0;
                for( j = 0; j < d.NUM_HH_INCOME; j++ )
                {
                  ptr[j] = tempMGRA[k,j];
                  tSum += ptr[j];
                }  // end for j
                if( mgraHHI[k] < 0 && tSum < Math.Abs( mgraHHI[k] ) )
                {
                  MessageBox.Show( "sp2 won't converge - tsum < rowMarginal" );
                }  // end if
                ret = UDMUtils.specialPachinko2( ptr, ref mgraHHI[k], luzHHI, d.NUM_HH_INCOME );
                if( ret > 0 && ret != 9999 )
                {
                  MessageBox.Show( "sp2 failed LUZ " + ( i + 1 ) + " mgraId " +  ( mgraId + 1 ) );
                }  // end if
                else if( ret == 99999 )
                {
                  MessageBox.Show( "Failed to find sp2 pachinko distribution!" );
                } // end else if

                  // Restore
                for( j = 0; j < d.NUM_HH_INCOME; j++ )
                  tempMGRA[k,j] = ptr[j];
              }     // End if
            }     // End for k

              // Compute col sums of MGRA data
            for( j = 0; j < d.NUM_HH_INCOME; j++ )
            {
              colTotal[j] = 0;
              for( k = 0; k < nmgras; k++ )
                colTotal[j] += tempMGRA[k,j];
            }  // end for j

            if( MINC_DEBUG )
              //printMInc( mInc, i, tempMGRA, ptmgraId, colTotal, nmgras );

            //if( MINC_DEBUG )
              //d.writeToStatusBox( "      rem rows" );
        
              // Now do remaining rows MGRA change.
            for( k = 0; k < nmgras; k++ )
            {
              mgraId = ptmgraId[k];
              for( j = 0; j < d.NUM_HH_INCOME; j++ )
                ptr[j] = tempMGRA[k,j];
              if( mgraHHI[k] > 0 )
              {
                ret = UDMUtils.specialPachinko3( ptr, ref mgraHHI[k], luzHHI, d.NUM_HH_INCOME );
                if( ret > 0 && ret != 99999 )
                {
                  MessageBox.Show( "sp3 failed LUZ " + ( i + 1 ) + " mgraId  " + ( mgraId + 1 ) );

                } // end if
                else if( ret == 9999 )
                {
                  MessageBox.Show( "Failed to find sp3 pachinko distribution!" );

                }  // end else if 

                  // Restore
                for( j = 0; j < d.NUM_HH_INCOME; j++ )
                  tempMGRA[k,j] = ptr[j];
              }     // End if
            }     // End for k

              // Compute col sums of MGRA data
            for( j = 0; j < d.NUM_HH_INCOME; j++ )
            {
              colTotal[j] = 0;
              for( k = 0; k < nmgras; k++ )
                colTotal[j] += tempMGRA[k,j];
            }  // end for j

            if( MINC_DEBUG )
              printMInc( d,mInc, i, tempMGRA, ptmgraId, colTotal, nmgras );

              // Replace data in m structure
            for( k = 0; k < nmgras; k++ )
            {
              mgraId = ptmgraId[k];
              for( j = 0; j < d.NUM_HH_INCOME; j++ )
              {
                mbData[mgraId].fcst.i.hh[j] = tempMGRA[k,j];
                mbData[mgraId].fcst.ii.hh[j] = mbData[mgraId].fcst.i.hh[j] - mbData[mgraId].baseData.i.hh[j];
              }  // end for j
            }     // End for k
          }     // End for i
      
          if( MINC_DEBUG )
            mInc.Close();
        }     // End method mgraCalcIncome()

      /*****************************************************************************/

      /* method mgraCalcCiv() */
      /// <summary>
      /// Method to compute MGRA civilian employment.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/16/97   tb    Initial coding
       *                 09/05/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcCiv( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int k;
          d.writeToStatusBox( "Computing MGRA civ.." );
          for( k = 0; k < d.NUM_MGRAS; k++ )
          {
            mbData[k].fcst.e.adj = mbData[k].baseData.e.adj + mbData[k].fcst.ei.adj;
            mbData[k].fcst.e.mil = mbData[k].baseData.e.mil + mbData[k].fcst.ei.mil;
          }
            // Control the MGRA civ emp LUZ totals
          //mgraControlCiv( d, mbData, z );
        }     // End method mgraCalcCiv()

      /*****************************************************************************/

      /* method mgraCalcSectors() */
      /// <summary>
      /// Method to compute and control MGRA employment sector distribution.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 07/21/98   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraCalcSectors( Detailed d, MBMaster[] m, Master[] z )
        {
          StreamWriter mEmp = null;
          int i, j, k;
          int mgraID;
          int ret;
          int tSum;
          int nMGRAs;
          int[] colTotal = new int[d.NUM_EMP_SECTORS];
          int[] mgraEI = new int[d.MAX_MGRAS_IN_LUZ];     // Row marginal
          int[] luzEI = new int[d.NUM_EMP_SECTORS];
          int[] ptc = new int[d.MAX_MGRAS_IN_LUZ];
          int[] ptr = new int[d.NUM_EMP_SECTORS];
          int[] ptmgraID = new int[d.MAX_MGRAS_IN_LUZ];
          int[,] tempMGRA = new int[d.MAX_MGRAS_IN_LUZ, d.NUM_EMP_SECTORS];

          if( MEMP_DEBUG )
          {
            try
            {
              mEmp = new StreamWriter(new FileStream(string.Format(d.appSettings["mgraEmp"].Value), FileMode.Create));
              mEmp.AutoFlush = true;
            }
            catch( IOException e )
            {
              MessageBox.Show( e.ToString(), "IOException" );
              Application.Exit();
            }
          }

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            //int zii = 0;
            //if (i == 28)
            //  zii = 1;
            nMGRAs = 0;

              /* Fill the temp array with MGRA emp data.  This simplifies passing data to pachinko. */
            for( k = 0; k < d.NUM_MGRAS; k++ )
            {
                // Find the LUZ for this MGRA
              if( m[k].luz - 1 == i )
              {
                  // Store the MGRA IDs
                ptmgraID[nMGRAs] = k;
                for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
                {
                  // Move the emp sector distribution data to temp array
                  tempMGRA[nMGRAs,j] = m[k].baseData.e.sectors[j] + m[k].site.sectors[j];
                  // Get LUZ col totals as sum across MGRAs from base year data
                  colTotal[j] += tempMGRA[nMGRAs,j];
                }  //end for
                nMGRAs++;     // Increment the MGRA counter for this LUZ
              }  // endif   
            }     // End for k

            /* tempMGRA now stores the base emp + sitespec distribution for the
              * MGRAs in this LUZ.  colTotal stores base emp + sitespec
              * distribution totals */

            /* Do any negative LUZ column totals - see doc on LUZ decrements.
              * Pachinko arguments are LUZ column marginals and MGRA HH marginals
              */

              // Load row and col marginals
            for( k = 0; k < nMGRAs; k++ )
            {
              mgraID = ptmgraID[k];
              mgraEI[k] = m[mgraID].fcst.ei.adj;      /* MGRA civ adjusted for sitespec change */
            }  // end for

              // Load LUZ sectors distribution for col marginals
            for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
              luzEI[j] = z[i].fcst.ei.sectorsAdj[j];

              // Start controlling here
            for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
            {
              for( k = 0; k < nMGRAs; k++ )
                ptc[k] = tempMGRA[k,j];
                // Look for LUZ emp category decrements
              if( z[i].fcst.ei.sectorsAdj[j] < 0 )
              {
                  // Call special pachinko 1 for negative LUZ changes
           
                ret = UDMUtils.specialPachinko1( ptc, mgraEI, ref luzEI[j], nMGRAs );
                if( ret > 0 && ret != 9999 )
                {
                  MessageBox.Show( "specialPachinko1 failed LUZ " + ( i + 1 ) + " sector " + ( j + 1 ) );
             
                }  // end if
                else if( ret == 9999 )
                {
                  MessageBox.Show( "Failed to find specialPachinko1 distribution!" );
              
                }  // end else if
                  // Restore
                for( k = 0; k < nMGRAs; k++ )
                  tempMGRA[k,j] = ptc[k];
              }     // End if

                // Compute col sums of MGRA data
              for( k = 0; k < nMGRAs; k++ )
                colTotal[j] += tempMGRA[k,j];
            }     // End for j
        
            // Now do rows with negative MGRA change
            for( k = 0; k < nMGRAs; k++ )
            {
              mgraID = ptmgraID[k];
              if( mgraEI[k] < 0 )
              {
                tSum = 0;
                for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
                {
                  ptr[j] = tempMGRA[k,j];
                  tSum += ptr[j];
                }  // end for
                if( mgraEI[k] < 0 && tSum < Math.Abs( mgraEI[k] ) )
                {
                  MessageBox.Show( "specialPachinko2 won't converge :  tSum < rowMarginal" + ( i + 1 ) + " mgraID " + ( mgraID + 1 ) );

                }  // end if
                ret = UDMUtils.specialPachinko2( ptr, ref mgraEI[k], luzEI,d.NUM_EMP_SECTORS );
                if( ret > 0 && ret != 9999 )
                {
                  MessageBox.Show( "specialPachinko2 failed LUZ " + ( i + 1 ) + " mgraID " + ( mgraID + 1 ) );

                } // end if
                else if( ret == 9999 )
                {
                  MessageBox.Show( "Failed to find specialPachinko2 distribution." + ( i + 1 ) + " mgraID " + ( mgraID + 1 ) );

                } // end else if
                  // Restore
                for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
                  tempMGRA[k,j] = ptr[j];
              }     // End if
            }     // End for k
          
              // Compute col sums of MGRA data
            for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
            {
              colTotal[j] = 0;
              for( k = 0; k < nMGRAs; k++ )
                colTotal[j] += tempMGRA[k,j];
            }  // end for
        
            // Now do remaining rows MGRA change
            // check for remaining column marginals
            Boolean goodluzEI = false;
            for (j = 0; j < d.NUM_EMP_SECTORS; ++j)
            {
              if (luzEI[j] > 0)
              {
                goodluzEI = true;
                break;
              }  // end if
            }  //end for

            if (goodluzEI)
            {

              for (k = 0; k < nMGRAs; k++)
              {

                mgraID = ptmgraID[k];
                for (j = 0; j < d.NUM_EMP_SECTORS; j++)
                  ptr[j] = tempMGRA[k, j];
                if (mgraEI[k] > 0)
                {
                  ret = UDMUtils.specialPachinko3(ptr, ref mgraEI[k], luzEI, d.NUM_EMP_SECTORS);
                  if (ret > 0 && ret != 9999)
                  {
                    MessageBox.Show("specialPachinko3 failed LUZ " + (i + 1) + " mgraID " + (mgraID + 1));

                  }  // end if
                  else if (ret == 9999)
                  {
                    MessageBox.Show("Failed to find specialPachinko3 distribution! LUZ " + (i + 1) + " mgraID " + (mgraID + 1));

                  }  // end else if
                  // Restore
                  for (j = 0; j < d.NUM_EMP_SECTORS; j++)
                    tempMGRA[k, j] = ptr[j];
                }     // End if
              }     // End for k
            } // end if goodluzEI

            // Compute col sums of MGRA data
            for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
            {
              colTotal[j] = 0;
              for( k = 0; k < nMGRAs; k++ )
                colTotal[j] += tempMGRA[k,j];
            }  // end for

              // Replace data in m array
            for( k = 0; k < nMGRAs; k++ )
            {
              mgraID = ptmgraID[k];
              m[mgraID].fcst.e.civ = 0;
              m[mgraID].fcst.e.mil = m[mgraID].baseData.e.mil;
              for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
              {
                m[mgraID].fcst.e.sectors[j] = tempMGRA[k,j];
                m[mgraID].fcst.ei.sectors[j] = m[mgraID].fcst.e.sectors[j] - m[mgraID].baseData.e.sectors[j];
                m[mgraID].fcst.e.civ += m[mgraID].fcst.e.sectors[j];
              }  // end for
              m[mgraID].fcst.e.total = m[mgraID].fcst.e.civ + m[mgraID].fcst.e.mil;
            }     // End for k
          }     // End for i

          if( MEMP_DEBUG )
            mEmp.Close();
        }     // End method mgraCalcSectors()

      /*****************************************************************************/

      /* method mgraControlCiv() */
      /// <summary>
      /// Method to control the MGRA civ emp estimates to LUZ totals.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/16/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraControlCiv( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int luzID;
          int mgraID;
          int ret;
          int j,i;
          int[,] tmgraAdj = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
          int[] pt = new int[d.MAX_MGRAS_IN_LUZ];
          int[] nmgra = new int[d.NUM_LUZS];
          int[,] mgraIDMaster = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];

          //d.writeToStatusBox( "Controlling MGRA civ.." );

            // Build temp arrays with units by LUZ and MGRA
          for( j = 0; j < d.NUM_MGRAS; j++ )
          {
            mgraID = j + 1;
            luzID = mbData[j].luz - 1;
            mgraIDMaster[luzID,nmgra[luzID]] = mgraID;
            tmgraAdj[luzID,nmgra[luzID]++] = mbData[j].fcst.e.adj;
          } // end for

          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            for( j = 0; j < nmgra[i]; j++ )
              pt[j] = tmgraAdj[i,j];

            ret = UDMUtils.roundItNoLimit( pt, z[i].fcst.e.civ, nmgra[i] );
            if( ret > 0 )
              MessageBox.Show( "mgraControlCiv - roundItNoLimit did not converge LUZ " + ( i + 1 ) );

              // Restore the rounded totals
            for( j = 0; j < nmgra[i]; j++ )
            {
              mgraID = mgraIDMaster[i,j] -1;
              mbData[mgraID].fcst.e.adj = pt[j];
            } // end for j
          }     // End for i
        }     // End method mgraControlCiv()

      /*****************************************************************************/

      /* method mgraControlER() */
      /// <summary>
      /// Method to control the MGRA ER estimates to LUZ totals.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/15/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraControlER( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int luzId;
          int mgraId;
          int nmc;
          int ret;
          int i,j;
          bool[] oFlag = new bool[d.MAX_MGRAS_IN_LUZ];
          int[] pt = new int[d.MAX_MGRAS_IN_LUZ];
          int[] ubound = new int[d.MAX_MGRAS_IN_LUZ];
          int[] nmgra = new int[d.NUM_LUZS];      
          int[,] tmgraER = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
          int[,] tmgraPop = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
          int[,] mgraIdMaster = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];

          //d.writeToStatusBox( "Controlling MGRA ER.." );

            // Build temp arrays with units by LUZ and MGRA.
          for ( j = 0; j < d.NUM_MGRAS; j++ )
          {
            luzId =  mbData[j].luz - 1;
            if (luzId > d.NUM_LUZS)
              MessageBox.Show("luzID exceed array bound");
            nmc = nmgra[luzId];
            mgraIdMaster[luzId,nmc] = j;
            tmgraER[luzId,nmc] = mbData[j].fcst.p.er;
            tmgraPop[luzId,nmc++] = mbData[j].fcst.p.pop;
            nmgra[luzId] = nmc;
          }  // end for j
            // Pass through each LUZ and control totals to LUZ HHP.
          for( i = 0; i < d.NUM_LUZS; ++i)
          { 
        
            for( j = 0; j < nmgra[i]; j++ )
            {
              pt[j] = tmgraER[i,j];
              ubound[j] = tmgraPop[i,j];
              oFlag[j] = false;     // Set override flags to false for controlling
            }  // end for j
            ret = UDMUtils.roundItUpperLimit( pt, oFlag, ubound, z[i].fcst.p.er,nmgra[i] );
            if( ret > 0 )
              MessageBox.Show( "mgraControlER - roundItUpperLimit did not converge LUZ " + ( i + 1 ) );
              // Restore the rounded totals
            for( j = 0; j < nmgra[i]; j++ )
            {
              mgraId = mgraIdMaster[i,j];
              mbData[mgraId].fcst.p.er = pt[j];
            }  // end for j
          }     // End for i
        }     // End method mgraControlER()

      /*****************************************************************************/

      /* method mgraControlGQ() */
      /// <summary>
      /// Method to control the MGRA GQ estimates to LUZ totals.  Uses pachinko on
      /// base GQ to control to LUZ forecast.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/15/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraControlGQ( Detailed d, MBMaster[] mbData, Master[] z )
        {
          int luzId;
          int mgraId;
          int j,i;
          int[,] tmgraGQ = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
          int[] pt = new int[d.MAX_MGRAS_IN_LUZ];
          int[] nmgra = new int[d.NUM_LUZS];
          int[,] mgraIdMaster = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];

          //d.writeToStatusBox( "Controlling LUZ GQs.." );

            // Build temp arrays with units by LUZ and MGRA
          for( j = 0; j < d.NUM_MGRAS; j++ )
          {
            luzId =  mbData[j].luz - 1;
            mgraIdMaster[luzId,nmgra[luzId]] = j;
            tmgraGQ[luzId,nmgra[luzId]++] = mbData[j].baseData.p.gqCiv;
          } // end for j

            // Pass through each LUZ and control totals to LUZ GQ.
          for( i = 0; i < d.NUM_LUZS; i++ )
          {
            for( j = 0; j < nmgra[i]; j++ )
              pt[j] = tmgraGQ[i,j];

            UDMUtils.pachinko( d, z[i].fcst.p.gqCiv, pt, nmgra[i] );

              // Restore the rounded totals
            for( j = 0; j < nmgra[i]; j++ )
            {
              mgraId = mgraIdMaster[i,j];
              mbData[mgraId].fcst.p.gqCiv = pt[j];
            }  // end for j
            z[i].fcst.p.gq = z[i].fcst.p.gqMil + z[i].fcst.p.gqCiv;
          }  // end for i
        }     // End method mgraControlGQ()

      /*****************************************************************************/

      /* method mgraControlHH() */
      /// <summary>
      /// Method to control the MGRA HH estimates to LUZ totals.
      /// </summary>
  
      /* Revision History
      * 
      * STR             Date       By    Description
      * --------------------------------------------------------------------------
      *                 12/15/97   tb    Initial coding
      *                 09/03/03   df    C# revision
      * --------------------------------------------------------------------------
      */
      public static void mgraControlHH( Detailed d, MBMaster[] mbData, Master[] z )
      {
        int i, j,k;
        int luzId;
        int mgraId;
        int realIndex;
        int ret = 0;
        int sum;
        int jj;

        int[,] tmgraHHSF = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHHMF = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHHMH = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHSSF = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHSMF = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHSMH = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[] nmgra = new int[d.NUM_LUZS];
        int[,] mgraIDMaster = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] dIndex1 = new int[d.MAX_MGRAS_IN_LUZ,2];
        int[] sortedData = new int[d.MAX_MGRAS_IN_LUZ];
        int[] ubound = new int[d.MAX_MGRAS_IN_LUZ];

        for (j = 0; j < d.NUM_LUZS; ++j)
        {
            for (k = 0; k < d.MAX_MGRAS_IN_LUZ; ++k)
            {
                mgraIDMaster[j, k] = new int();
                tmgraHHSF[j, k] = new int();
                tmgraHHMF[j, k] = new int();
                tmgraHHMH[j, k] = new int();
                tmgraHSSF[j, k] = new int();
                tmgraHSMF[j, k] = new int();
                tmgraHSMH[j, k] = new int();
            }  // end for k
        }   // end for j

        try
        {
            // Build temp arrays with units by LUZ and MGRA
            for (j = 0; j < d.NUM_MGRAS; j++)
            {
                luzId = mbData[j].luz - 1;
                jj = nmgra[luzId];
                mgraIDMaster[luzId, jj] = j;
                tmgraHHSF[luzId, jj] = mbData[j].fcst.hh.sf;
                tmgraHHMF[luzId, jj] = mbData[j].fcst.hh.mf;
                tmgraHHMH[luzId, jj] = mbData[j].fcst.hh.mh;
                tmgraHSSF[luzId, jj] = mbData[j].fcst.hs.sf;
                tmgraHSMF[luzId, jj] = mbData[j].fcst.hs.mf;
                tmgraHSMH[luzId, jj] = mbData[j].fcst.hs.mh;
                ++nmgra[luzId];
            }  // end for j
        }
        catch (Exception e)
        {
            MessageBox.Show(e.ToString(), e.GetType().ToString());
        }
          // Pass through each LUZ and control totals to LUZ unit counts.
        for( i = 0; i < d.NUM_LUZS; i++ )
        {
            if (i == 94)
                i = 94;
          // SF
          for( j = 0; j < nmgra[i]; j++ )
          {
            dIndex1[j,0] = j;
            dIndex1[j,1] = tmgraHHSF[i,j];
          }  // end for j      
          // Sort them
          UDMUtils.quickSort( dIndex1, 0, nmgra[i]-1 );
          sum = 0;
          for( j = 0; j < nmgra[i]; j++ )
          {
            sortedData[j] = dIndex1[j,1];
            sum += sortedData[j];
            realIndex = dIndex1[j,0];
            ubound[j] = tmgraHSSF[i,realIndex];
          }  // end for j
          if( sum > 0 && sum != z[i].fcst.hh.sf )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.sf,nmgra[i], 0 );
          else if( sum == 0 )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.sf,nmgra[i], 1 );
          if( ret > 0 )
            MessageBox.Show( "mgraControlHH roundIt did not converge, difference = " + ret + " LUZ " + ( i + 1 ) );

            // Restore the rounded totals
          for( j = 0; j < nmgra[i]; j++ )
          {
            realIndex = dIndex1[j,0];
            mgraId = mgraIDMaster[i,realIndex];
            mbData[mgraId].fcst.hh.sf = sortedData[j];
          }  // end for
      
          // MF
          for( j = 0; j < nmgra[i]; j++ )
          {
            dIndex1[j,0] = j;
            dIndex1[j,1] = tmgraHHMF[i,j];
          }   // end for
      
            // Sort them
          UDMUtils.quickSort( dIndex1, 0, nmgra[i] -1 );
          sum = 0;
          for( j = 0; j < nmgra[i]; j++ )
          {
            sortedData[j] = dIndex1[j,1];
            sum += sortedData[j];
            realIndex = dIndex1[j,0];
            ubound[j] = tmgraHSMF[i,realIndex];
          }   // end for

          if( sum > 0 && sum != z[i].fcst.hh.mf )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.mf, nmgra[i], 0 );
          else if( sum == 0 )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.mf, nmgra[i], 1 );
          if( ret > 0 )
            MessageBox.Show( "mgraControlHH roundit did not converge, difference = " + ret + " LUZ " + ( i + 1 ) );
            // Restore the rounded totals
          for( j = 0; j < nmgra[i]; j++ )
          {
            realIndex = dIndex1[j,0];
            mgraId = mgraIDMaster[i,realIndex];
            mbData[mgraId].fcst.hh.mf = sortedData[j];
          }   // end for
      
          // mh
          for( j = 0; j < nmgra[i]; j++ )
          {
            dIndex1[j,0] = j;
            dIndex1[j,1] = tmgraHHMH[i,j];
          }    // end for
    
          // Sort them
          UDMUtils.quickSort( dIndex1, 0, nmgra[i]-1 );
          sum = 0;

          for( j = 0; j < nmgra[i]; j++ )
          {
            sortedData[j] = dIndex1[j,1];
            sum += sortedData[j];
            realIndex = dIndex1[j,0];
            ubound[j] =tmgraHSMH[i,realIndex];
          }   // end for

          if( sum > 0 && sum != z[i].fcst.hh.mh )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.mh,nmgra[i], 0 );
          else if( sum == 0 )
            ret = UDMUtils.roundIt( sortedData, ubound, z[i].fcst.hh.mh,nmgra[i], 1 );
          if( ret > 0 )
            MessageBox.Show( "mgraControlHH roundIt did not converge, difference = " + ret + " LUZ " + ( i + 1 ) );
            // Restore the rounded totals
          for( j = 0; j < nmgra[i]; j++ )
          {
            realIndex = dIndex1[j,0];
            mgraId = mgraIDMaster[i,realIndex];
            mbData[mgraId].fcst.hh.mh = sortedData[j];
          }  // end for

          // HH total
          z[i].fcst.hh.total = z[i].fcst.hh.sf + z[i].fcst.hh.mf + z[i].fcst.hh.mh;
          z[i].fcst.hhi.total = z[i].fcst.hh.total - z[i].baseData.hh.total;
          z[i].fcst.hhi.sf = z[i].fcst.hh.sf - z[i].baseData.hh.sf;
          z[i].fcst.hhi.mf = z[i].fcst.hh.mf - z[i].baseData.hh.mf;
          z[i].fcst.hhi.mh = z[i].fcst.hh.mh - z[i].baseData.hh.mh;
        }     // End for i
      }     // End method mgraControlHH()

      /*****************************************************************************/

      /* method mgraControlHHP() */
      /// <summary>
      /// Method to control the MGRA HHP estimates to LUZ totals.
      /// </summary>
  
      /* Revision History
      * 
      * STR             Date       By    Description
      * --------------------------------------------------------------------------
      *                 12/15/97   tb    Initial coding
      *                 09/03/03   df    C# revision
      * --------------------------------------------------------------------------
      */
      public static void mgraControlHHP( Detailed d, MBMaster[] mbData,Master[]  z )
      {
        int luzId;
        int mgraId;
        int target;
        int sum;
        int diff;
        int ret,i,j;
        bool[] oFlag = new bool[d.MAX_MGRAS_IN_LUZ];      
        int[] pt = new int[d.MAX_MGRAS_IN_LUZ];
        int[] lbound = new int[d.MAX_MGRAS_IN_LUZ];
        int[] nmgra = new int[d.NUM_LUZS];
        int[,] mgraIdMaster = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHHP = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];
        int[,] tmgraHH = new int[d.NUM_LUZS,d.MAX_MGRAS_IN_LUZ];

        //d.writeToStatusBox( "Controlling HHP.." );

          // Build temp arrays with units by LUZ and MGRA
        for(j = 0; j < d.NUM_MGRAS; j++ )
        {
          luzId = mbData[j].luz - 1;
          mgraIdMaster[luzId,nmgra[luzId]] = j;
          tmgraHHP[luzId,nmgra[luzId]] = mbData[j].fcst.p.hhp;
            // Constrain HHP to >= HH
          tmgraHH[luzId,nmgra[luzId]++] = mbData[j].fcst.hh.total;
        } // end for j

          // Pass through each LUZ and control totals to LUZ HHP
        for(i = 0; i < d.NUM_LUZS; i++ )
        {
          sum = 0;      // Initialize the sum
          for (j = 0; j < d.MAX_MGRAS_IN_LUZ; ++j)
          {
            lbound[j] = 0;
            pt[j] = 0;
          } // end for j

          for(j = 0; j < nmgra[i]; j++ )
          {
            pt[j] = tmgraHHP[i,j];        // Fill the temp array with MGRA HHP
            lbound[j] = tmgraHH[i,j];     // Fill the lower bound array with HH
            sum += pt[j];                 // Get the sum
          }  // end for j
          target = z[i].fcst.p.hhp;
          diff = target - sum;
            // Go to the rounding routine if the summ != target
          if( diff != 0 )
          {
            ret = UDMUtils.roundItLowerLimit( pt, oFlag, lbound, z[i].fcst.p.hhp,nmgra[i] );
            if( ret > 0 )
              MessageBox.Show( "mgraControlHHP roundItLowerLimit did not converge LUZ " + ( i + 1 ) );
              // Restore the rounded totals
            for(j = 0; j < nmgra[i]; j++ )
            {
              mgraId = mgraIdMaster[i,j];
              mbData[mgraId].fcst.p.hhp = pt[j];
            }   // end for j
          }  // end if
        }     // End for i
      }     // End method mgraControlHHP()

      /*****************************************************************************/

      /* method mgraERRates() */
      /// <summary>
      /// Method to compute MGRA ER/HH.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/15/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraERRates( int j, double[] t25ER, MBMaster[] mbData,Master[] z )
        {
          int luzId;
            // Get base year rate
          if( mbData[j].baseData.hh.total > 0 )
            mbData[j].baseData.r.erHH = ( double )mbData[j].baseData.p.er / ( double )mbData[j].baseData.hh.total;
            // Derive forecast rate
          luzId = mbData[j].luz - 1;      // Get the LUZ id for this MGRA

            /* If the base year HH > 20, set the max of 0.5 or base * LUZ rate change. */
          if( mbData[j].baseData.hh.total > 20 )
            mbData[j].fcst.r.erHH = Math.Max( ( mbData[j].baseData.r.erHH * t25ER[luzId] ), 0.5 );
          else      // Otherwise use the LUZ rate
            mbData[j].fcst.r.erHH = z[luzId].fcst.r.erHH;
        }     // End method mgraERRates()

      /*****************************************************************************/

      /* method mgraHHSRates() */
      /// <summary>
      /// Method to compute MGRA HHS.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/15/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraHHSRates( MBMaster[] mbData, Master[] z, int j,double[]t24HHP )
        {
          int luzId;
            // Get base year rate
          if( mbData[j].baseData.hh.total > 0 )
            mbData[j].baseData.r.hhs = ( double )mbData[j].baseData.p.hhp /( double )mbData[j].baseData.hh.total;
            // Derive forecast rate
          luzId = mbData[j].luz - 1;      // Get the LUZ id for this MGRA

            // If the base year HH > 20, set the max of 1 or base * LUZ rate change
          if( mbData[j].baseData.hh.total > 20 )
            mbData[j].fcst.r.hhs = Math.Max( ( mbData[j].baseData.r.hhs * t24HHP[luzId] ), 1 );
          else      // Otherwise use the LUZ rate
            mbData[j].fcst.r.hhs = z[luzId].fcst.r.hhs;
        }     // End method mgraHHSRates()

      /*****************************************************************************/

      /* method mgraVacRates() */
      /// <summary>
      /// Method to compute MGRA vacancy rates.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/12/97   tb    Initial coding
       *                 09/03/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void mgraVacRates( int j, double[] t23sf, double[] t23mf,
                                         double[] t23mh, MBMaster[] mbData,
                                         Master[] z )
        {
          int luzId;
            // Get base year rate
          if( mbData[j].baseData.hs.sf > 0 )
            mbData[j].baseData.r.vSF = ( 1.0 - ( double )mbData[j].baseData.hh.sf /( double )mbData[j].baseData.hs.sf ) * 100;
          if( mbData[j].baseData.hs.mf > 0 )
            mbData[j].baseData.r.vMF = ( 1.0 - ( double )mbData[j].baseData.hh.mf /( double )mbData[j].baseData.hs.mf ) * 100;
          if( mbData[j].baseData.hs.mh > 0 )
            mbData[j].baseData.r.vmh = ( 1.0 - ( double )mbData[j].baseData.hh.mh /( double )mbData[j].baseData.hs.mh ) * 100;

            // Derive forecast rate
          luzId = mbData[j].luz - 1;      // Get the LUZ id for this MGRA

            // If the base year HS > 20, set the min of 1 or base * LUZ rate change
      
            // SF
          if( mbData[j].baseData.hs.sf > 20 )
            mbData[j].fcst.r.vSF = Math.Min( ( mbData[j].baseData.r.vSF * t23sf[luzId] ), 1 );
          else      // Otherwise use the LUZ rate
            mbData[j].fcst.r.vSF = z[luzId].fcst.r.vSF;

            // MF
          if( mbData[j].baseData.hs.mf > 20 )
            mbData[j].fcst.r.vMF = Math.Min( ( mbData[j].baseData.r.vMF * t23mf[luzId] ), 1 );
          else      // Otherwise use the LUZ rate
            mbData[j].fcst.r.vMF = z[luzId].fcst.r.vMF;

            // mh
          if( mbData[j].baseData.hs.mh > 20 )
            mbData[j].fcst.r.vmh = Math.Min( ( mbData[j].baseData.r.vmh * t23mh[luzId] ), 1 );
          else      // Otherwise use the LUZ rate
            mbData[j].fcst.r.vmh = z[luzId].fcst.r.vmh;
        }     // End method mgraVacRates()

      /*****************************************************************************/

      /* method printMEmp() */
      /// <summary>
      /// Method to perform debug printing for MGRA sector calculations.
      /// </summary>
      /// <param name="arr">MGRA income array</param>
      /// <param name="cTot">Column totals</param>
      /// <param name="id">MGRA IDs</param>
  
      /* Revision History
      * 
      * STR             Date       By    Description
      * --------------------------------------------------------------------------
      *                 07/21/98   tb    Initial coding
      *                 09/04/03   df    C# revision
      * --------------------------------------------------------------------------
      */
      private static void printMEmp( Detailed d, StreamWriter mEmp, int i, int[,] arr,
                                      int[] id, int[] cTot, int counter )
      {
        int rowTotal;
        string head1 = "MGRA       1     2     3     4     5     6     7     8     9    10   11    12    13    14   TOT";
        string head2 = "-----------------------------------------------------------------------------------------------";
        mEmp.WriteLine( "LUZ " + ( i + 1 ) );
        mEmp.WriteLine( head1 );
        mEmp.WriteLine( head2 );

        for( int k = 0; k < counter; k++ )
        {
          rowTotal = 0;
          mEmp.Write( "{0,6}", ( id[k] + 1 ).ToString() );
          for( int j = 0; j < d.NUM_EMP_SECTORS; j++ )
          {
            rowTotal += arr[k,j];
            mEmp.Write( "{0,6}", arr[k,j].ToString() );
          }
          mEmp.WriteLine( "{0,6}", rowTotal );
        }
        mEmp.Write( "TOTAL " );
        for( int j = 0; j < d.NUM_EMP_SECTORS; j++ )
          mEmp.Write( "{0,6}", cTot[j] );      
        mEmp.WriteLine();
        mEmp.WriteLine();
        mEmp.WriteLine();
        mEmp.WriteLine( head2 );
      }     // End method printMEmp()

      /*****************************************************************************/

      /* method printMInc() */
      /// <summary>
      /// Method to handle debug printing for MGRA income calculations.
      /// </summary>
      /// <param name="arr">MGRA income array</param>
      /// <param name="ctot">Column totals</param>
      /// <param name="id">MGRA IDs array</param>

      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 07/20/98   tb    Initial coding
       *                 09/04/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void printMInc( Detailed d, StreamWriter mInc, int i, int[,] arr,
                                      int[] id, int[] ctot, int counter )
        {
          int rowTotal;
          string head1 = "MGRA       1     2     3     4     5     6     7     8     9    10   TOT";
          string head2 = "------------------------------------------------------------------------";
                     
          mInc.WriteLine( "LUZ " + ( i + 1 ) );
          mInc.WriteLine( head1 );
          mInc.WriteLine( head2 );
      
          for( int k = 0; k < counter; k++ )
          {
            rowTotal = 0;
            mInc.Write( "{0,6}", id[k] + 1 );
            for( int j = 0; j < d.NUM_HH_INCOME; j++ )
            {
              rowTotal += arr[k,j];
              mInc.Write( "{0,6}", arr[k,j] );
            }
            mInc.WriteLine( "{0,6}", rowTotal );
          }
          mInc.Write( "TOTAL " );
          for( int j = 0; j < d.NUM_HH_INCOME; j++ )
            mInc.Write( "{0,6}", ctot[j] );
          mInc.WriteLine();
          mInc.WriteLine();
          mInc.WriteLine();
          mInc.WriteLine( head2 );
        }     // End method printMInc()

      /*****************************************************************************/

      /* method rebuildLUZBase() */
      /// <summary>
      /// Method to rebuild luzBase from mgraBase data before writing luzBase to
      /// ASCII.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/17/97   tb    Initial coding
       *                 09/04/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void rebuildLUZBase( Detailed d,MBMaster m, Master[] zt,Master [] z)
        {
          int j;
          int luzID = m.luz - 1;
          if (m.luz == 18)
            j = 0;
          // Cycle through the luzBase vars
          // Pop variables
      
          zt[luzID].fcst.p.pop += m.fcst.p.pop;
          zt[luzID].fcst.p.hhp += m.fcst.p.hhp;
          zt[luzID].fcst.p.gqCiv += m.fcst.p.gqCiv;
          zt[luzID].fcst.p.gqMil += m.fcst.p.gqMil;
          zt[luzID].fcst.p.er += m.fcst.p.er;

            // HS
          zt[luzID].fcst.hs.total += m.fcst.hs.total;
          zt[luzID].fcst.hs.sf += m.fcst.hs.sf;
          zt[luzID].fcst.hs.mf += m.fcst.hs.mf;
          zt[luzID].fcst.hs.mh += m.fcst.hs.mh;
      
            // HH
          zt[luzID].fcst.hh.total += m.fcst.hh.total;
          zt[luzID].fcst.hh.sf += m.fcst.hh.sf;
          zt[luzID].fcst.hh.mf += m.fcst.hh.mf;
          zt[luzID].fcst.hh.mh += m.fcst.hh.mh;

            // Get income parameters from standard LUZ array
          zt[luzID].fcst.i.median = z[luzID].fcst.i.median;
          zt[luzID].fcst.nla = z[luzID].fcst.nla;
          zt[luzID].fcst.asd = z[luzID].fcst.asd;

            // Income
          for( j = 0; j < d.NUM_HH_INCOME; j++ )
            zt[luzID].fcst.i.hh[j] += m.fcst.i.hh[j];

            // Employment
          zt[luzID].fcst.e.total += m.fcst.e.total;
          zt[luzID].fcst.e.civ += m.fcst.e.civ;
          zt[luzID].fcst.e.mil += m.fcst.e.mil;
     
          for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
            zt[luzID].fcst.e.sectors[j] += m.fcst.e.sectors[j];
        }     // End method rebuildLUZBase()

      /*****************************************************************************/

      /* method writeMB() */
      /// <summary>
      /// Method to write mbData structure to ASCII.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/17/97   tb    Initial coding
       *                 09/04/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void writeMB(Detailed d, MBMaster[] m, Master[] zt,Master[] z, int scenarioID, int increment )
        {
          StreamWriter tempMB = null;
          int i, j;
          string xx;
          //----------------------------------------------------------------------
          xx = "";
          try
          {
            tempMB = new StreamWriter(new FileStream(d.networkPath + String.Format(d.appSettings["tempMB"].Value),FileMode.Create ) );
            tempMB.AutoFlush = true;
          }
          catch( IOException e )
          {
            MessageBox.Show( e.ToString(), "IOExeption" );
            Application.Exit();
          }

          d.writeToStatusBox( "Writing MGRABase to ASCII.." );
          for( i = 0; i < d.NUM_MGRAS; i++ )
          {
       
              // Rebuild the luzbase records from the MGRAs
            //rebuildLUZBase( m[i], zt,z);

              // Rebuild totals
              // Employment
            m[i].fcst.e.total = m[i].fcst.e.civ + m[i].fcst.e.mil;      
        
              // MGRA and LUZ
            xx = scenarioID + "," + increment + "," + (i+1).ToString() + "," + m[i].luz.ToString() + ",";
      
              // Pop housing data
            xx = xx + m[i].fcst.p.pop.ToString() + "," + 
                      m[i].fcst.p.hhp.ToString() + "," +
                      m[i].fcst.p.er.ToString() + "," +
                      m[i].fcst.p.gq.ToString()+ "," +
                      m[i].fcst.p.gqCiv.ToString() + "," + 
                      m[i].fcst.p.gqCivCol + "," + 
                      m[i].fcst.p.gqCivOth + "," +  
                      m[i].fcst.p.gqMil.ToString() + "," +
                      m[i].fcst.hs.total.ToString() + "," +
                      m[i].fcst.hs.sf.ToString() + "," +
                      m[i].fcst.hs.mf.ToString() + "," +
                      m[i].fcst.hs.mh.ToString() + "," +
                      m[i].fcst.hh.total.ToString() + "," +
                      m[i].fcst.hh.sf.ToString() + "," +
                      m[i].fcst.hh.mf.ToString() + "," +
                      m[i].fcst.hh.mh.ToString() + ",";

              // Employment data
            xx = xx + m[i].fcst.e.total.ToString() + "," +
                      m[i].fcst.e.civ.ToString() + "," +
                      m[i].fcst.e.mil.ToString() + ",";
                
            for( j = 0; j < d.NUM_EMP_SECTORS; j++ )
              xx = xx + m[i].fcst.e.sectors[j].ToString() + ",";

            for(j = 0; j < 4; ++j)
              xx = xx + m[i].fcst.empLU[j].ToString() + ",";
        
              // Income data
            for( j = 0; j < d.NUM_HH_INCOME; j++ )
              xx = xx + m[i].fcst.i.hh[j].ToString() + "," ;
        
              // Land use data - dev acres; +dev_mil, dev_water, dev_mixed use 
            for( j = 0; j <= 49; j++ )
              xx = xx + m[i].fcst.acres[j].ToString() + ",";
       
            xx = xx + m[i].fcst.acres[50].ToString();

            tempMB.WriteLine(xx);
          }     // End for i

          tempMB.Close();
          
        }     // End method writeMB()

      /*****************************************************************************/

      /* method writeZB() */
      /// <summary>
      /// Method to write luzBase structures to ASCII.
      /// </summary>
  
      /* Revision History
       * 
       * STR             Date       By    Description
       * --------------------------------------------------------------------------
       *                 12/17/97   tb    Initial coding
       *                 09/04/03   df    C# revision
       * --------------------------------------------------------------------------
       */
        public static void writeZB( Detailed d, Master[] zt, int scenarioID, int increment )
        {
          string xx;
          StreamWriter tempZB = null;
          try
          {
            tempZB = new StreamWriter(new FileStream(d.networkPath + String.Format(d.appSettings["tempZB"].Value), FileMode.Create));
           
            tempZB.AutoFlush = true;
          }
          catch( IOException e )
          {
            MessageBox.Show( e.ToString(), "IOException" );
            Application.Exit();
          }

          d.writeToStatusBox( "Writing luzBase next table to ASCII.." );
          for( int i = 0; i < d.NUM_LUZS; i++ )
          {
              // Population variables
            xx = scenarioID + "," + increment + "," + (i+1).ToString() + "," +
                       zt[i].fcst.p.pop.ToString() + "," +
                       zt[i].fcst.p.hhp.ToString() + "," +
                       zt[i].fcst.p.er.ToString() + "," +
                       zt[i].fcst.p.gq.ToString() + "," +
                       zt[i].fcst.p.gqCiv.ToString() + "," +
                       zt[i].fcst.p.gqMil.ToString()+"," +
                       zt[i].fcst.p.gqCivCol + "," +
                       zt[i].fcst.p.gqCivOth + ","; 
                       

              // HS
            xx = xx + zt[i].fcst.hs.total.ToString() + "," +
                      zt[i].fcst.hs.sf.ToString() + "," +
                      zt[i].fcst.hs.mf.ToString() + "," +
                      zt[i].fcst.hs.mh.ToString() + ",";

              // HH
            xx = xx + zt[i].fcst.hh.total.ToString() + "," +
                      zt[i].fcst.hh.sf.ToString() + "," +
                      zt[i].fcst.hh.mf.ToString() + "," +
                      zt[i].fcst.hh.mh.ToString() + ",";
      
              // Income parameters and dist
            xx = xx + zt[i].fcst.i.median.ToString() + "," +
                      zt[i].fcst.asd.ToString() + "," +
                      zt[i].fcst.nla.ToString() + ",";

            for( int j = 0; j < d.NUM_HH_INCOME; j++ )
              xx = xx + zt[i].fcst.i.hh[j].ToString() + ",";

            xx = xx + zt[i].fcst.e.total.ToString() + "," +
                      zt[i].fcst.e.civ.ToString() + "," +
                      zt[i].fcst.e.mil.ToString() + ",";
                
            for( int j = 0; j < d.NUM_EMP_SECTORS-1; j++ )
              xx = xx + zt[i].fcst.e.sectors[j].ToString() + ",";
            xx = xx + zt[i].fcst.e.sectors[d.NUM_EMP_SECTORS-1].ToString();
            tempZB.WriteLine(xx);
          }     // End for i
      
          tempZB.Close();
        }     // End method writeZB()

        //***************************************************************

        /* procedure writeLUZHistory() */

        /* write luz history structures to ascii */

        /* Revision History
            STR            Date       By   Description
             -------------------------------------------------------------------------
                           12/24/97   tb   initial coding
             -------------------------------------------------------------------------
        */

        /*----------------------------------------------------------------------------*/

        public static void writeLUZHistory(Detailed d,Master[] z, int scenarioID, int increment)
        {
          int i;

          string xx;
          StreamWriter tempZH = null;
          try
          {
              tempZH = new StreamWriter(new FileStream(d.networkPath + String.Format(d.appSettings["tempZH"].Value), FileMode.Create));
           
            tempZH.AutoFlush = true;
          }
          catch( IOException e )
          {
            MessageBox.Show( e.ToString(), "IOException" );
            Application.Exit();
          }
          for (i = 0; i < d.NUM_LUZS; ++i)
          {
            xx = scenarioID + "," + increment + ","+ (i+1).ToString() + "," +
                z[i].histEmp.L5.ToString() + "," +
                z[i].histEmp.L0.ToString() + "," +
                z[i].histSF.L5.ToString() + "," +
                z[i].histSF.L0.ToString() + "," +
                z[i].histMF.L5.ToString() + "," +
                z[i].histMF.L0.ToString() + "," +
                z[i].histHH.L5.ToString() + "," +
                z[i].histHH.L0.ToString();
            tempZH.WriteLine(xx);
          }     /* end for i */

          tempZH.Close();

        }     /* end procedure writeLUZHistory() */

        /******************************************************************************/

        /* procedure updateLUZHistory()*/

        /* update the luz_history hs and emp arrays */
        /* move the 5-year data to the 10 and the base to the 5
           and replace the base with the forecast */

        /* Revision History
             STR            Date       By   Description
             -------------------------------------------------------------------------
                            07/08/97   tb   initial coding
             -------------------------------------------------------------------------
        */
        /*---------------------------------------------------------------------------*/

        public static void updateLUZHistory(Detailed d,Master[] z)
        {
          int i;

          /*-------------------------------------------------------------------------*/

          for (i = 0; i < d.NUM_LUZS; ++i)
          {
            /* move the data down */
            z[i].histSF.L5 = z[i].histSF.L0;
            z[i].histMF.L5 = z[i].histMF.L0;
            z[i].histHH.L5 = z[i].histHH.L0;
            z[i].histEmp.L5 = z[i].histEmp.L0;

            /* replace the base data */
            z[i].histSF.L0 = z[i].fcst.hs.sf;
            z[i].histMF.L0 = z[i].fcst.hs.mf;
            z[i].histHH.L0 = z[i].fcst.hh.total;
            z[i].histEmp.L0 = z[i].fcst.e.civ;
          }     /* end for i */

        }     /* end procedure updateLUZHistory()*/

        /******************************************************************************/

	}     // End class DCUtils
}     // End namespace udm.dcutils
