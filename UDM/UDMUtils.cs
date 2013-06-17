/* Filename:    UDMUtils.cs
 * Program:     UDM
 * Version:     7.0 sr13
 * Programmers: Terry Beckhelm
 *              Daniel Flyte (C# revision)
 * Description: Set of utility methods which all modules call.
 * 
 * Included procedures
 *  assignParameters()
 *  buildBaseRatios()
 *  checkCapacity()
 *  checkTableExists()
 *  copyCapacity()
 *  errorFunction()
 *  extractHistory()
 *  extractLUZBase()
 *  extractLUZTemp()
 *  getArrayStats()
 *  getArraySum()
 *  histChg()
 *  inAg()
 *  inCommercial()
 *  inDevParks()
 *  inIndustrial()
 *  inOffice()
 *  inOther()
 *  inSchools()
 *  inWater()
 *  medianIncome()
 *  moveV()
 *  pachinko()
 *  pachinkoNeg()
 *  quickSort()
 *  roundIt()
 *  roundItLowerLimit()
 *  roundItNeg()
 *  roundItNoLimit()
 *  roundItUpperLimit()
 *  specialPachinkoEmp()
 *  specialPachinko1()
 *  specialPachinko2()
 *  specialPachinko3()
 *  storeZB()
 *  storeZH()
 */


using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Sandag.TechSvcs.RegionalModels
{
  public class UDMUtils
  {
    /*****************************************************************************/

    

    /* method buildBaseRatios() */
    /// <summary>
    /// Method to create base emp and hh ratios for employment equation.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/27/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static void buildBaseRatios(BaseForm form,Master[] z )
    {
      int rege =  0;
      int regh = 0;
      int i;  
      for(i = 0; i < form.NUM_LUZS; i++ )
      {
        // Compute regional totals for hh and base civ emp
        rege += z[i].baseData.e.civ;
        regh += z[i].histHH.L0;
      }   // end for

      for(i = 0; i < form.NUM_LUZS; i++ )
      {
        // Compute ratios for hh and base civ emp
        if( rege > 0 )
          z[i].fcst.civR = ( double )z[i].baseData.e.civ / ( double )rege;
        if( regh > 0 )
          z[i].fcst.hhR = ( double )z[i].histHH.L0 / ( double )regh;
      }   // end for i        
    }     // End method buildBaseRatios()

    /*****************************************************************************/

    /* method checkCapacity() */
    /// <summary>
    /// Method to check for capacity to continue; 1 = emp, 2 = hsSF, 3 = hsMF
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 01/22/02   tb    Initial coding
    *                 07/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool checkCapacity( BaseForm form, int type , int scenarioID, int bYear)
    {
          SqlDataReader rdr;
      
          int by = 0, fy = 0, capCheck = 0, fcstCheck = 0, year1 = 0;
          string cmd1 = "", cmd2 = "";
          // ---------------------------------------------------------------------
            
          switch( type )
          {
            case 1:     // Employment
              cmd1 = "SELECT SUM(cap_emp_civ) FROM " + form.TN.capacity + " WHERE scenario = " + scenarioID + " AND increment = " + bYear;
              cmd2 = "SELECT year, civ FROM " + form.TN.regfcst + " WHERE scenario = " + scenarioID;
              break;

            case 2:     // hsMF
              cmd1 = "SELECT SUM(cap_hs_sf) FROM " + form.TN.capacity + " WHERE scenario = " + scenarioID + " AND increment = " + bYear;
              cmd2 = "SELECT year, hs_sf FROM " + form.TN.regfcst + " WHERE scenario = " + scenarioID;
              break;

            case 3:     // hsMF
              cmd1 = "SELECT SUM(cap_hs_mf) FROM " + form.TN.capacity + " WHERE scenario = " + scenarioID + " AND increment = " + bYear;
              cmd2 = "SELECT year, hs_mf FROM " + form.TN.regfcst + " WHERE scenario = " + scenarioID;
              break;
          }   // end switch

          form.sqlCommand.CommandText = cmd1;
          try
          {
            form.sqlConnection.Open();
            rdr = form.sqlCommand.ExecuteReader();
            while (rdr.Read())
              capCheck = rdr.GetInt32(0);
            rdr.Close();
       
          }   // end try
          catch (Exception s)
          {
            MessageBox.Show(s.Message, "SQL Exception");
          }
          finally
          {
            form.sqlConnection.Close();
          }

          form.sqlCommand.CommandText = cmd2;
          try
          {
		    form.sqlConnection.Open();
            rdr = form.sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
              year1 = rdr.GetInt16( 0 );
              fcstCheck = rdr.GetInt32( 1 );
              if( year1 == form.bYear )
                by = fcstCheck;
              else if( year1 == form.fYear )
                fy = fcstCheck;
            }   // end while()
            rdr.Close();
			
          }   // end try
          catch( SqlException s )
          {
            MessageBox.Show( s.Message, "SQL Exception" );
          }   // end catch
          finally
          {
            form.sqlConnection.Close();
          }
          fcstCheck = fy - by;
      
          if( capCheck > fcstCheck )
              return true;
          return false;
    }     // End method checkCapacity()

    /*****************************************************************************/

    /* method copyCapacity() */
    /// <summary>
    /// Method to duplicate the capacity table with inserts.
    
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/21/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    *                 10/13/09   tb    added code to apply record-level negative employment overrides, plus
    *                                  changing records that have devcodes 13 or 14 - something to roads; these records have 
     *                                 historically been skipped in earlier versions of UDM; we just caught them now;  they 
     *                                 will be treated like sitespec records- have to occur in appropriate phase; see notes below for detail
    * --------------------------------------------------------------------------
    */
    public static bool copyCapacity( BaseForm form, int sw , int scenarioID, int bYear,int fYear)
    {
        string ctext1 = "";
        string ctext2 = "";

        form.sqlCommand.CommandTimeout = 180;
        form.writeToStatusBox( "Building capacityNext table.." );      
        switch( sw )
        {
            case 1:     // Copying base year capacity to capacity1 for module 1        
                ctext1 = String.Format(form.appSettings["deleteFrom"].Value, form.TN.capacity1, scenarioID, bYear);
                ctext2 = String.Format(form.appSettings["insertFrom"].Value, form.TN.capacity1, form.TN.capacity, scenarioID, bYear);
                break;

            case 2:     // Copying module 1 capacity to TN.capacity2 for module 2
                form.writeToStatusBox( "Deleting from capacity table for module 2.." ); 
                ctext1 = String.Format(form.appSettings["deleteFrom"].Value, form.TN.capacity2, scenarioID, bYear);
                ctext2 = String.Format(form.appSettings["insertFrom"].Value, form.TN.capacity2, form.TN.capacity1, scenarioID, bYear);
                break;          
          
            case 3:     // Copying module 2 capacity to TN.capacity3 for module 3
                ctext1 = String.Format(form.appSettings["deleteFrom"].Value, form.TN.capacity3, scenarioID, bYear);               
                ctext2 = String.Format(form.appSettings["insertFrom"].Value, form.TN.capacity3, form.TN.capacity2, scenarioID, bYear);
                break;
  
            case 4:     // Copying module 3 capacity to capacity4 for module 4
                ctext1 = String.Format(form.appSettings["deleteFrom"].Value, form.TN.capacity4, scenarioID, bYear);
                ctext2 = String.Format(form.appSettings["insertFrom"].Value, form.TN.capacity4, form.TN.capacity3, scenarioID, bYear);
                break;

            case 5:     // Copying module 4 capacity to final capacityNext
                ctext1 =  String.Format(form.appSettings["deleteFrom"].Value, form.TN.capacity, scenarioID, fYear);             
                
                ctext2 = String.Format(form.appSettings["insert01"].Value, form.TN.capacity, fYear,form.TN.capacity4, scenarioID, bYear);
                break;
            
        }     // End switch

        // execute the delete command
        form.sqlCommand.CommandText = ctext1;
        try
        {
            form.sqlConnection.Open();
            form.sqlCommand.ExecuteNonQuery();
        }
        catch (Exception sq)
        {
             MessageBox.Show(sq.ToString(), "SQL Exception");
            return false;
        }
        finally
        {
            form.sqlConnection.Close();
        }

        // execute the insert command
        form.sqlCommand.CommandText = ctext2;
        try
        {
            form.sqlConnection.Open();
            form.sqlCommand.ExecuteNonQuery();
        }
        catch (Exception sq)
        {
            MessageBox.Show(sq.ToString(), "SQL Exception");
            return false;
        }
        finally
        {
            form.sqlConnection.Close();
        }

        if (sw == 1) // update initial capacity copy for initial values
        {
            form.sqlCommand.CommandText = String.Format(form.appSettings["update01"].Value, form.TN.capacity1, scenarioID, bYear);
            try
            {
                form.sqlConnection.Open();
                form.sqlCommand.ExecuteNonQuery();
            }
            catch (Exception sq)
            {
                MessageBox.Show(sq.ToString(), "SQL Exception");
                return false;
            }
            finally
            {
                form.sqlConnection.Close();
            }
        }  // endif
      
      return true;
    }     // End method copyCapacity()

    /*****************************************************************************/

    /* method errorFunction() */
    /// <summary>
    /// error function for income distribution equation
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/04/95   tb    Initial coding
    *                 08/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static double errorFunction( double y )
    {
      int i;
      int iSw;
      int iSkip;
      double x, res, xSq = 0, xNum, xDen, xi;
          
      //     COEFFICIENTS FOR 0.0 <= y < .477
      double[] p = {113.8641541510502, 377.4852376853020, 3209.377589138469,0.1857777061846032, 3.181123743870566};
      double[] q = {244.0246379344442, 1282.616526077372, 2844.236833439171,23.60129095234412};
      double[] p1 = {8.883149794388376, 66.11919063714163, 298.6351381974001,881.9522212417691, 1712.047612634071, 2051.078377826071,
                      1230.339354797997, 2.153115354744038E-8,0.5641884969886701};
      double[] q1 = {117.6939508913125, 537.1811018620099, 1621.389574566690,3290.799235733460, 4362.619090143247, 3439.367674143722,
                      1230.339354803749, 15.74492611070983};

      // COEFFICIENTS FOR 4.0 < y
      double[] p2 = {-3.603448999498044E-01, -1.257817261112292E-01,-1.608378514874228E-02, -6.587491615298378E-04,
                      -1.631538713730210E-02, -3.053266349612323E-01};
      double[] q2 = {1.872952849923460,    5.279051029514284E-01,6.051834131244132E-02,  2.335204976268692E-03, 2.568520192289822};
      double xMin = 1.0E-10;
      double xLarge = 6.375;
      double sqrPi = 0.5641895835477563;
      // --------------------------------------------------------------------

      iSkip = 0;
      x = y;
      iSw = 1;
      if( x < 0.0 )
      {
        iSw = -1;
        x = -x;
      }   // end if
      if( x < 0.477 )
      {
        if( x >= xMin )
        {
          xSq = x * x;
          xNum = p[3] * xSq + p[4];
          xDen = xSq + q[3];
          for( i = 0; i < 3; i++ )
          {
            xNum = xNum * xSq + p[i];
            xDen = xDen * xSq + q[i];
          }   // end for i
          res = x * ( xNum / xDen );
        }   // end if
        else
          res = x * ( p[2] / q[2] );
        iSkip = 1;
      }   // end if

      else if( x <= 4.0 )
      {
        xSq = x * x;
        xNum = p1[7] * x + p1[8];
        xDen = x + q1[7];
        for( i = 0; i < 7; i++ )
        {
          xNum = xNum * x + p1[i];
          xDen = xDen * x + q1[i];
        }   // end for i
        res = xNum / xDen;
      }   // end else i

      else if( x < xLarge )
      {
        xSq = x * x;
        xi = 1.0 / xSq;
        xNum = p2[4] * xi + p2[5];
        xDen = xi + q2[4];
        for( i = 0; i < 4; i++ )
        {
          xNum = xNum * xi + p2[i];
          xDen = xDen * xi + q2[i];
        }   // end for i
        res = ( sqrPi + xi * ( xNum / xDen ) ) / x;
      }   // end else if

      else 
      {
        res = 1.0;
        iSkip = 1;
      }

      if( iSkip == 0 )
      {
        res = res * Math.Exp( -xSq );
        res = 1.0 - res;
      }

      if( iSw == -1 )
        res = -res;

      return ( double)res;
    }     // End method errorFunction()

    /*****************************************************************************/

    /* method extractHistory() */
    /// <summary>
    /// Method to build LUZ emp and hs history array.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/10/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool extractHistory( BaseForm form , int scenarioID, int bYear)
    {
        int zi, luzb;
        int[] e = new int[2];
        int[] s = new int[2];
        int[] ma = new int[2];
        int[] h = new int[2];
        int i;
      
        SqlDataReader rdr;

        // ----------------------------------------------------------------------
        // Reset status box text
        form.writeToStatusBox( "Extracting LUZ history data.." );
      
        form.sqlCommand.CommandText = String.Format(form.appSettings["selectAll"].Value,form.TN.luzhist,scenarioID,bYear);

        try
        {
		    form.sqlConnection.Open();
	
            rdr = form.sqlCommand.ExecuteReader();
            while( rdr.Read() )
            {
                luzb = rdr.GetInt16( 2 );  // skip scenario and increment
                e[0] = rdr.GetInt32( 3 );  
                e[1] = rdr.GetInt32( 4 );
                s[0] = rdr.GetInt32( 5 );
                s[1] = rdr.GetInt32( 6 );
                ma[0] = rdr.GetInt32( 7 );
                ma[1] = rdr.GetInt32( 8 );
                h[0] = rdr.GetInt32( 9 );
                h[1] = rdr.GetInt32( 10 );
                zi = luzb - 1;
                form.z[zi] = new Master();
                // Store the query results in zh struct
                storeZH( form.reg, form.z[zi], e, s, ma, h );
            }   // end while
		    rdr.Close();
	
          }   // end try
          catch( Exception sq )
          {
                MessageBox.Show( sq.ToString(), "SQL Exception" );
                return false;
          }   // end catch

          finally
          {
            form.sqlConnection.Close();
          }  

          // Derive changes and % changes
          histChg( form.reg, form.z,form.NUM_LUZS );

          string path = "";
          path = form.networkPath + form.TN.luzhist;
          if (form.ZH_DEBUG )
          {
            StreamWriter zh = new StreamWriter( new FileStream(path, FileMode.Create ) );
            for(i = 0; i < form.NUM_LUZS; i++ )
            {
              zh.WriteLine( "{0,4}{1,8}{2,8}{3,8}{4,8}{5,8}{6,8}{7,8}{8,8}{9,8}{10,8}{11,8}{12,8}", i + 1, form.z[i].histEmp.L5, 
                            form.z[i].histEmp.L0, form.z[i].histEmp.c5, form.z[i].histSF.L5, form.z[i].histSF.L0, 
                            form.z[i].histSF.c5, form.z[i].histMF.L5, form.z[i].histMF.L0, form.z[i].histMF.c5, 
                            form.z[i].histHH.L5, form.z[i].histHH.L0, form.z[i].histHH.c5 );
              zh.Flush();
            }
            zh.Close();
          }   // end if
     
          return true;
    }     // End method extractHist()

    /*****************************************************************************/

    /* method extractLUZBase() */
    /// <summary>
    /// Method to get LUZBase data for civilian employment.  Returns true if 
    /// method executed properly, otherwise returns false and program will exit 
    /// thereafter.
    /// </summary>
    /// 

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/21/03   df    C# revision
    *                 12/27/05   tb    added extracting home price index table
    * --------------------------------------------------------------------------
    */
    public static bool extractLUZBase(BaseForm form, int[] zbi, int scenarioID, int bYear)
    {
        int i, j, zi,iter;
        double f1, f2;
        double[] rat = new double[10];
	    SqlDataReader rdr;
        string str = "";
        StreamWriter zb;
        // -----------------------------------------------------------------------  
      
     
        string zHead1 = " LUZ     Pop     HHP      ER  GQ-CIV  GQ-MIL   HS_SF    HS_MF   HS_mh   HH_SF   HH_MF   HH_mh";
        string zHead2 = " LUZ    <15k   15-30   30-45   45-60   60-75   75-99 100-125 125-150 150-200    200+";
        string zHead3 = " LUZ     Emp     Civ     Mil      Ag    Cons     Mfg    Whtrd   Retrd    Twu    Info    Fre    Pbs   EdHs     Os     Gov    SEDW";
        str = form.networkPath + String.Format(form.appSettings["luzBaseCheck"].Value);
        zb = new StreamWriter(new FileStream(str, FileMode.Create));

        if(form.ZH_DEBUG )     // Debug trace
        {
        
            zb.WriteLine( "LUZBASE INPUT CHECK" );
            zb.Flush();
        }   // end if
      form.writeToStatusBox( "Extracting LUZBase Data.." );

      form.sqlCommand.CommandText = String.Format(form.appSettings["selectAll"].Value, form.TN.luzbase, scenarioID, bYear);
      
      try
      {
		form.sqlConnection.Open();
	
        rdr = form.sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
            iter = 2;  // skip scenario and increment
            form.luzB = rdr.GetInt16( iter++ );
        
            // Filling zbi is broken up due to differing source data types.
            for( i = 0; i < 17; i++ )
                zbi[i] = rdr.GetInt32( iter++);

            f1 = rdr.GetDouble( iter++ );
            f2 = rdr.GetDouble( iter++ );
            
            for( i = 17; i < 50; i++ )
                zbi[i] = rdr.GetInt32( iter++ );
        
            zi = form.luzB - 1;
            storeZB( form.minSwitch, zi, zbi, form.minParam, form.reg, form.z,form.NUM_EMP_SECTORS );
           
            /* These income curve parameters are stored in the forecast structure. */
            form.z[zi].fcst.asd = f1;
            form.z[zi].fcst.nla = f2;

            // Derive vacancy rate
            if( form.z[zi].baseData.hs.total > 0 )
                form.z[zi].baseData.r.v = ( 1.0 - ((double )form.z[zi].baseData.hh.total / ( double )form.z[zi].baseData.hs.total ) ) * 100;
            else
                form.z[zi].baseData.r.v = 0;
        }   // end while
        rdr.Close();
		
      }     // End try

      catch( Exception sq )
      {
        MessageBox.Show( sq.ToString(), "SQL Exception" );
        return false;
      }   // end try
      finally
      {
        form.sqlConnection.Close();
      } 

      form.writeToStatusBox( "Extracting LUZ Home Price Index data.." );
        
      form.sqlCommand.CommandText = String.Format(form.appSettings["selectSimple"].Value,form.TN.homePrices);
      
      try
      {
        form.sqlConnection.Open();
			
        rdr = form.sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          iter = 0;
          form.luzB = rdr.GetInt16( iter++ );
          zi = form.luzB - 1;
          form.z[zi].homePriceIndex = rdr.GetDouble(iter++);
        }   // end while
        rdr.Close();
			
      }     // End try

      catch( Exception sq )
      {
        MessageBox.Show( sq.ToString(), "SQL Exception" );
        return false;
      }   // end try
      finally
      {
        form.sqlConnection.Close();
      }
  
      if( form.ZB_DEBUG )     // Debug trace
      {
        zb.WriteLine( zHead1 );
        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          zb.WriteLine( "{0,4}{1,8}{2,8}{3,8}{4,8}{5,8}{6,8}{7,8}{8,8}{9,8}" +
                        "{10,8}{11,8}", i + 1, form.z[i].baseData.p.pop, form.z[i].baseData.p.hhp, form.z[i].baseData.p.er, 
                        form.z[i].baseData.p.gqCiv, form.z[i].baseData.p.gqMil, form.z[i].baseData.hs.sf, 
                        form.z[i].baseData.hs.mf, form.z[i].baseData.hs.mh, form.z[i].baseData.hh.sf, form.z[i].baseData.hh.mf, 
                        form.z[i].baseData.hh.mh );
          zb.Flush();
        }   // end for i

        zb.WriteLine();
        zb.WriteLine();
        zb.WriteLine( zHead2 );
        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          zb.Write( "{0,4}", i + 1 );
          for( j = 0; j < 10; j++ )
            zb.Write( "{0,8}", form.z[i].baseData.i.hh[j] );
          zb.WriteLine();
          zb.Flush();
        }   // end for i

        zb.WriteLine();
        zb.WriteLine();
        zb.WriteLine( zHead3 );
        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          zb.Write( "{0,4}{1,8}{2,8}{3,8}", i + 1, form.z[i].baseData.e.total, form.z[i].baseData.e.civ, form.z[i].baseData.e.mil );
          for( j = 0; j < form.NUM_EMP_SECTORS; j++ )
            zb.Write( "{0,8}", form.z[i].baseData.e.sectors[j] );
          zb.WriteLine();
          zb.Flush();
        }   // end for i
        zb.Close();
      }   // End if ZBDEBUG

      form.writeToStatusBox( "Extracting LUZ income adjustment parameters data.." );
      form.sqlCommand.CommandText = String.Format(form.appSettings["select07"].Value, form.TN.luzIncomeParms);
      try
      {
		form.sqlConnection.Open();
	
        rdr = form.sqlCommand.ExecuteReader();     
        while( rdr.Read() )
        {
          form.luzB = rdr.GetInt16( 0 );
          for( i = 0; i < form.NUM_HH_INCOME; i++ )
            rat[i] = rdr.GetDouble( i + 1 );
        
          zi = form.luzB - 1;
          for( i = 0; i < form.NUM_HH_INCOME; i++ )
            form.z[zi].fcst.incomeAdj[i] = rat[i];
        }   // end while
			  rdr.Close();
	
      }   // end try
      catch( Exception sq )
      {
          MessageBox.Show( sq.ToString(), "SQL Exception" );
          return false;
      }
      finally
      {
        form.sqlConnection.Close();
      }  

      /* At this point all luzs have been loaded and the regional data have been accumulated in store_zb.  However we need to compute regional 
        * vacancy rate and median income. */

      // Vacancy
      if( form.reg.baseData.hs.total > 0 )
        form.reg.baseData.r.v = ( 1.0 - (( double )form.reg.baseData.hh.total /( double )form.reg.baseData.hs.total ) ) * 100;
      else
        form.reg.baseData.r.v = 0;

      // Median income
      form.reg.baseData.i.median = medianIncome( form.reg.baseData.i.hh );
      
      return true;
    }     // End method extractLUZBase()

    /*****************************************************************************/

    /* method extractLUZTemp() */
    /// <summary>
    /// Method to populate LUZ aggregation from temp table.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/21/97   tb    Initial coding
    *                 07/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool extractLUZTemp( BaseForm form , int scenarioID, int bYear)
    {
      StreamWriter ztt = null;
      int i, j, typeB;
      string zhead1 = " LUZ       Civ       Mil        SF        MF        mh    GQ-Civ    GQ_MIL";
      string zhead2 = " LUZ     Redev    Infill   Vac-Ind   Vac-Com   Vac-Off   Vac-Sch     Total";
      string zhead3 = " LUZ     Redev    Infill   Vac-Low   Vac-Urb     Total";
      string zhead4 = " LUZ     Redev    Infill    Vac-Ag     Total";

      int[] sumi = new int[20];
      int[] v1b = new int[6];
      int[] v3b = new int[17];
      double[] sumf = new double[20];
      double[] v2b = new double[17];
      double[] v4b = new double[17]; 
      
      SqlDataReader rdr;
      // ---------------------------------------------------------------------
      
      if( form.ZT_DEBUG )
      {
        ztt = new StreamWriter( new FileStream( form.networkPath + String.Format(form.appSettings["luzTempCheck.txt"].Value),FileMode.Create ) );
        ztt.WriteLine( "LUZ TEMP INPUT CHECK" );
        ztt.WriteLine();
        ztt.Flush();
      }   // end if
    
      form.writeToStatusBox( "Extracting LUZ data from temp table.." );
      form.sqlCommand.CommandText = String.Format(form.appSettings["selectAll"].Value, form.TN.luztemp, scenarioID, bYear);
    
      try
      {
		form.sqlConnection.Open();
	
        rdr = form.sqlCommand.ExecuteReader();
        while( rdr.Read() )
        {
          typeB = rdr.GetInt32( 2 );  // skip scenario and increment
          form.luzB = rdr.GetInt16( 3 );
          for( i = 0; i < v1b.Length; i++ )
            v1b[i] = rdr.GetInt32( i + 4 );
        
          for( i = 0; i < v2b.Length; i++ )
            v2b[i] = rdr.GetDouble( i + 10 );

          for( i = 0; i < v3b.Length; i++ )
            v3b[i] = rdr.GetInt32( i + 27 );

          for( i = 0; i < v4b.Length; i++ )
            v4b[i] = rdr.GetDouble( i + 44 );
        
          i = form.luzB - 1;
          if( typeB == 1 )      // LUZs
            moveV( form.z[i], v1b, v2b, v3b, v4b );

          else if( typeB == 3 )     // Region
            moveV( form.reg, v1b, v2b, v3b, v4b );
        }   // end while
			  rdr.Close();

      }     // End try

      catch( Exception sq )
      {
        MessageBox.Show( sq.ToString(), "SQL Exception" );
        return false;
      }   // end catch
      finally
      {
        form.sqlConnection.Close();
      } 

      // Accumulate regional totals for group quarters.
      for( i = 0; i < form.NUM_LUZS; i++ )
      {
        form.reg.site.gqCiv += form.z[i].site.gqCiv;
        form.reg.site.gqMil += form.z[i].site.gqMil;
      }   // end for

      if( form.ZT_DEBUG )
      {
        ztt.WriteLine( "SITE SPEC DATA" );
        ztt.WriteLine();
        ztt.WriteLine( zhead1 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.WriteLine( "{0,4}{1,10}{2,10}{3,10}{4,10}{5,10}{6,10}", 
                      i + 1, form.z[i].site.civ, form.z[i].site.sf, form.z[i].site.mf, form.z[i].site.mh, form.z[i].site.gqCiv, form.z[i].site.gqMil );
          sumi[0] += form.z[i].site.civ;
          sumi[1] += form.z[i].site.sf;
          sumi[2] += form.z[i].site.mf;
          sumi[3] += form.z[i].site.mh;
          sumi[4] += form.z[i].site.gqCiv;
          sumi[5] += form.z[i].site.gqMil;
          ztt.Flush();
        }   // end for

        ztt.Write( "REG " );
        for( j = 0; j < 6; j++ )
          ztt.Write( "{0,10}", sumi[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "SITE SPEC ACRES - EMP" );
        ztt.WriteLine();

        ztt.WriteLine( zhead2 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_EMP_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].site.ac.ae[j] );
            sumf[j] += form.z[i].site.ac.ae[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].site.ac.totalEmpAcres );
          sumf[7] += form.z[i].site.ac.totalEmpAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 8; j++ )
          ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();

        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "SITE SPEC ACRES - SF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead3 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_SF_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].site.ac.asf[j] );
            sumf[j] += form.z[i].site.ac.asf[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].site.ac.totalSFAcres );
          sumf[5] += form.z[i].site.ac.totalSFAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 6; j++ )
            ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "SITE SPEC ACRES - MF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead4 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_MF_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].site.ac.amf[j] );
            sumf[j] += form.z[i].site.ac.amf[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].site.ac.totalMFAcres );
          sumf[4] += form.z[i].site.ac.totalMFAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 5; j++ )
          ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE - EMP" );
        ztt.WriteLine();
        ztt.WriteLine( zhead2 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_EMP_LAND; j++ )
          {
            ztt.Write( "{0,10}", form.z[i].capacity.e[j]);
            sumi[j] += form.z[i].capacity.e[j];
          }   // end for j
          ztt.WriteLine( "{0,10}", form.z[i].capacity.totalEmp );
          sumi[7] += form.z[i].capacity.totalEmp;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 8; j++ )
          ztt.Write( "{0,10}", sumi[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE - SF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead3 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_SF_LAND; j++ )
          {
            ztt.Write( "{0,10}", form.z[i].capacity.sf[j] );
            sumi[j] += form.z[i].capacity.sf[j];
          }   // end for j
          ztt.WriteLine( "{0,10}", form.z[i].capacity.totalSF );
          sumi[5] += form.z[i].capacity.totalSF;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 6; j++ )
          ztt.Write( "{0,10}", sumi[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE - MF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead4 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_MF_LAND; j++ )
          {
            ztt.Write( "{0,10}", form.z[i].capacity.mf[j] );
            sumi[j] += form.z[i].capacity.mf[j];
          }   // end for j
          ztt.WriteLine( "{0,10}", form.z[i].capacity.totalMF );
          sumi[4] += form.z[i].capacity.totalMF;
          ztt.Flush();
        }   // end for i

        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.Write( "REG " );
        for( j = 1; j < 5; j++ )
          ztt.Write( "{0,10}", sumi[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE ACRES - EMP" );
        ztt.WriteLine();
        ztt.WriteLine( zhead2 );
      
        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_EMP_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].capacity.ac.ae[j] );
            sumf[j] += form.z[i].capacity.ac.ae[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].capacity.ac.totalEmpAcres );
          sumf[7] += form.z[i].capacity.ac.totalEmpAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 8; j++ )
          ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE ACRES - SF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead3 );

        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_SF_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].capacity.ac.asf[j] );
            sumf[j] += form.z[i].capacity.ac.asf[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].capacity.ac.totalSFAcres );
          sumf[5] += form.z[i].capacity.ac.totalSFAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 6; j++ )
          ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();

        ztt.WriteLine( "CAPACITY BY LAND USE ACRES - MF" );
        ztt.WriteLine();
        ztt.WriteLine( zhead4 );
          
        for( i = 0; i < form.NUM_LUZS; i++ )
        {
          ztt.Write( "{0,4}", i + 1 );
          for( j = 1; j < form.NUM_MF_LAND; j++ )
          {
            ztt.Write( "{0,10:F2}", form.z[i].capacity.ac.amf[j] );
            sumf[j] += form.z[i].capacity.ac.amf[j];
          }   // end for j
          ztt.WriteLine( "{0,10:F2}", form.z[i].capacity.ac.totalMFAcres );
          sumf[4] += form.z[i].capacity.ac.totalMFAcres;
          ztt.Flush();
        }   // end for i

        ztt.Write( "REG " );
        for( j = 1; j < 5; j++ )
          ztt.Write( "{0,10:F2}", sumf[j] );
        ztt.Flush();
        ztt.WriteLine();
        ztt.WriteLine();
        ztt.WriteLine();
          ztt.Close();
      }     // End if ZT_DEBUG      
      
      return true;
    }     // End method extractLUZTemp()

    /*****************************************************************************/

    /* method getArrayStats() */
    /// <summary>
    /// Method to perform array distribution and cumulative probability used in 
    /// pachinko process
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/21/97   tb    Initial coding
    *                 07/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private static void getArrayStats( int[] arr, int localSum, double[] arrDist, double[] arrProb,int numCols )
    {
      double sum = 0;
      int i;
      // --------------------------------------------------------------------
      for(i = 0; i < numCols; i++ )
      {
        arrDist[i] = ( double )arr[i] / ( double )localSum * 100;
        sum += arrDist[i];
        arrProb[i] = sum;
      }   // end for i
    }     // End method getArrayStats()

    /*****************************************************************************/

    /* method getArraySum() */
    /// <summary>
    /// Method to sum array used in pachinko method.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/26/97   tb    Initial coding
    *                 07/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    private static int getArraySum( int[] array , int numCols )
    {
      int sum = 0;
      for( int i = 0; i < numCols; i++ )
        sum += array[i];
      return sum;
    }     // End method getArraySum()

    /*****************************************************************************/

    /* method histChg() */
    /// <summary>
    /// Method to compute luz history change variables.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static void histChg( Master reg, Master[] z , int NUM_LUZS)
    {
      int i;
      // Compute the changes for the region        
      // Emp 
      reg.histEmp.c5 = reg.histEmp.L0 - reg.histEmp.L5;     // 5 year change
      // sf
      reg.histSF.c5 = reg.histSF.L0 - reg.histSF.L5;
      // mf
      reg.histMF.c5 = reg.histMF.L0 - reg.histMF.L5;
      // hh
      reg.histHH.c5 = reg.histHH.L0 - reg.histHH.L5;

      for(i = 0; i < NUM_LUZS; i++ )
      {
        // Employment     5 year change
        z[i].histEmp.c5 = z[i].histEmp.L0 - z[i].histEmp.L5;
        // sf
        z[i].histSF.c5 = z[i].histSF.L0 - z[i].histSF.L5;
        // mf
        z[i].histMF.c5 = z[i].histMF.L0 - z[i].histMF.L5;
        // hh
        z[i].histHH.c5 = z[i].histHH.L0 - z[i].histHH.L5;
      }   // end for i
    }     // End method histChg()

    /*****************************************************************************/

    /* method inAg() */
    /// <summary>
    /// Method to check whether land is in agriculture land category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/03/98   tb    Initial coding
    *                 07/30/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inAg( int lu )
    {
      return( (lu >= 8000 && lu <= 8003 ) || (lu == 2201 || lu == 2301));
    }     // End method inAg()

    /*****************************************************************************/

    /* method inCommercial() */
    /// <summary>
    /// Method to check whether land is in commercial category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/03/98   tb    Initial coding
    *                 07/30/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inCommercial( int lu )
    {
      return( ( lu >= 1500 && lu <= 1503 ) || lu == 5000 || 
              ( lu >= 5002 && lu <= 5009 ) || ( lu >= 6100 && lu <= 6509 ) ||
              ( lu >= 7200 && lu <= 7211 ) );
    }     // End method inCommercial()

    /*****************************************************************************/

    /* method inDevParks() */
    /// <summary>
    /// Method to check whether land is in developed parks category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/06/98   tb    Initial coding
    *                 08/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inDevParks( int lu )
    {
      return ( (lu >= 7600 && lu <= 7609 ) || lu == 9300 || lu == 9400);
    }     // End method inDevParks()

    /*****************************************************************************/

    /* method inIndustrial() */
    /// <summary>
    /// Method to check whether land is in industrial category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/03/98   tb    Initial coding
    *                 07/30/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inIndustrial( int lu )
    {
      return( ( lu >= 2001 && lu <= 2105 ) || ( lu >= 4100 && lu <= 4111 ) ||
              ( lu >= 4113 && lu <= 4116 ) || lu == 4119 || lu == 4120 || 
              lu == 5001 );
    }     // End method inIndustrial()

    /*****************************************************************************/

    /* method inOffice() */
    /// <summary>
    /// Method to check whether land is in office category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/03/98   tb    Initial coding
    *                 07/30/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inOffice( int lu )
    {
      return( lu >= 6000 && lu <= 6003 );
    }     // End method inOffice()

    /*****************************************************************************/

    /* method inOther() */
    /// <summary>
    /// Method to check whether land is in other category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/06/98   tb    Initial coding
    *                 08/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inOther( int lu )
    {
      return ( lu >= 1400 && lu <= 1409 );
    }     // End method inOther()

    /*****************************************************************************/

    /* method inSchools() */
    /// <summary>
    /// Method to check whether land is in schools category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/03/98   tb    Initial coding
    *                 08/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inSchools( int lu )
    {
      return ( lu >= 6800 && lu <= 6809 );
    }     // End procedure inSchools()

    /*****************************************************************************/

    /* procedure insertsort()*/

    /* sort a small list */

    /* Revision History
     STR            Date       By   Description
     -------------------------------------------------------------------------
                    05/02/98   tb   initial coding
     -------------------------------------------------------------------------
    */
    /*---------------------------------------------------------------------------*/

    public static void insertsort(int [,] v,int n)
    {
      int i,j,temp0v,temp1v;
      for (i = 1; i < n; i++)
      {
        temp0v = v[i,0];
        temp1v = v[i,1];
        for (j = i-1; j >= 0 && v[j,1] < temp1v; j--)
        {
          v[j+1,0] = v[j,0];
          v[j+1,1] = v[j,1];
        }
        v[j+1,1] = temp1v;
        v[j+1,0] = temp0v;
      }     /* end for i */
    }     /* end procedure insertsort()*/

    /******************************************************************************/
    
    /* method inWater() */
    /// <summary>
    /// Method to check whether land is in water category.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 04/06/98   tb    Initial coding
    *                 08/22/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static bool inWater( int lu )
    {
      return ( lu >= 9200 && lu <= 9202 );
    }     // End method inWater()

    /*****************************************************************************/

    /* method medianIncome() */
    /// <summary>
    /// Method to perform median income calculations.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/09/96   tb    Initial coding
    *                 07/23/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int medianIncome( int[] data )
    {
      int sx = 0;
      double xp = 0;
      double sx1 = 0;
      double median = 0;
      double total = 0;
      int[] bounds = {0,15000,30000,45000,60000,75000,100000,125000,150000,200000,350000};
      int i;
      // ----------------------------------------------------------------------
      for(i = 0; i < 10; i++ )
        total += data[i];

      if( total == 0 )      // If total is 0, return median of 0.
        return 0;

      xp = ( double )( total * 50 ) / 100;

      for(i = 1; i < 11; i++ )
      {
        sx += data[i - 1];
        sx1 = data[i - 1] - sx + xp;
        if( xp - sx < 0 )
        {
          median = sx1 / ( double )data[i - 1] * 
                ( double )( bounds[i] - bounds[i - 1] ) + 
                ( double )bounds[i - 1];
          break;
        }   // end if
        else if( xp - sx == 0 )
        {
          median = ( double )bounds[i];
          break;
        }   // end else
      }   // end for i
      return ( int )median;
    }     // End method medianIncome()

    /*****************************************************************************/

    /* method moveV() */
    /// <summary>
    /// Method to store LUZ agg data from temp table into local array.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/21/97   tb    Initial coding
    *                 07/28/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static void moveV( Master zl, int[] v1, double[] v2, int[] v3, double[] v4 )
    {
      zl.site.civ = v1[0];
      zl.site.sf = v1[1];
      zl.site.mf = v1[2];
      zl.site.mh = v1[3];
      zl.site.gqCiv = v1[4];
      zl.site.gqMil = v1[5];
      zl.site.ac.ae[0] = v2[0];
      zl.site.ac.ae[1] = v2[1];
      zl.site.ac.ae[2] = v2[2];
      zl.site.ac.ae[3] = v2[3];
      zl.site.ac.ae[4] = v2[4];
      zl.site.ac.ae[5] = v2[5];
      zl.site.ac.ae[6] = v2[6];
      zl.site.ac.totalEmpAcres = v2[7];
      zl.site.ac.asf[1] = v2[8];
      zl.site.ac.asf[2] = v2[9];
      zl.site.ac.asf[3] = v2[10];
      zl.site.ac.asf[4] = v2[11];
      zl.site.ac.totalSFAcres = v2[12];
      zl.site.ac.amf[1] = v2[13];
      zl.site.ac.amf[2] = v2[14];
      zl.site.ac.amf[3] = v2[15];
      zl.site.ac.totalMFAcres = v2[16];
      zl.capacity.e[0] = v3[0];
      zl.capacity.e[1] = v3[1];
      zl.capacity.e[2] = v3[2];
      zl.capacity.e[3] = v3[3];
      zl.capacity.e[4] = v3[4];
      zl.capacity.e[5] = v3[5];
      zl.capacity.e[6] = v3[6];
      zl.capacity.totalEmp = v3[7];
      zl.capacity.sf[1] = v3[8];
      zl.capacity.sf[2] = v3[9];
      zl.capacity.sf[3] = v3[10];
      zl.capacity.sf[4] = v3[11];
      zl.capacity.totalSF = v3[12];
      zl.capacity.mf[1] = v3[13];
      zl.capacity.mf[2] = v3[14];
      zl.capacity.mf[3] = v3[15];
      zl.capacity.totalMF = v3[16];
      zl.capacity.ac.ae[0] = v4[0];
      zl.capacity.ac.ae[1] = v4[1];
      zl.capacity.ac.ae[2] = v4[2];
      zl.capacity.ac.ae[3] = v4[3];
      zl.capacity.ac.ae[4] = v4[4];
      zl.capacity.ac.ae[5] = v4[5];
      zl.capacity.ac.ae[6] = v4[6];
      zl.capacity.ac.totalEmpAcres = v4[7];
      zl.capacity.ac.asf[1] = v4[8];
      zl.capacity.ac.asf[2] = v4[9];
      zl.capacity.ac.asf[3] = v4[10];
      zl.capacity.ac.asf[4] = v4[11];
      zl.capacity.ac.totalSFAcres = v4[12];
      zl.capacity.ac.amf[1] = v4[13];
      zl.capacity.ac.amf[2] = v4[14];
      zl.capacity.ac.amf[3] = v4[15];
      zl.capacity.ac.totalMFAcres = v4[16];
    }     // End method moveV()

    /*****************************************************************************/

    /* method pachinko() */
    /// <summary>
    /// Method to perform +1 distribution scheme using a random number and 
    /// cumulative distribution to assign values to elements in an array.
    /// </summary>
    /// <param name="numCols">Number of categories to allocate</param>
    /// <param name="slave">Target area array to receive distribution</param>
    /// <param name="target">Control total of target assignment</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/26/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int pachinko( BaseForm form, int target, int[] slave,int numCols)
    {
      int i1,i;
      int loopCount = 0;
      double where;
      int localSum = 0;
      int diff;

      // Computed distribution, max 20 elements
      double[] localDist = new double[form.MAX_TRANS];
      // Cumulative probability
      double[] cumProb = new double[form.MAX_TRANS];
      Random rand = new Random(0);
      // -------------------------------------------------------------------------
      // Reset the computed arrays
    
      // Keep doing this until the target pop is met
      for(i = 0; i < numCols; i++ )
        localSum += slave[i];
      diff = target - localSum;
      while( diff != 0 && loopCount < 10000 )
      {
        getArrayStats( slave, localSum, localDist, cumProb, numCols );
        /* Get the random number between 1 and 10000 and convert to xx.xx 
        * decimal*/
  
        where = ( rand.NextDouble() * 100 );
        i1 = 9999;
        // Look for the index of the cumProb <= the random number
        for(i = 0; i < numCols; i++ )
        {
          if( where <= cumProb[i] )
          {
            i1 = i;      // Save the index in the cumProb
            break;
          }   // end if
        }   // end for i
        
        if( i1 == 9999 )
        {
            MessageBox.Show( "FATAL ERROR - failed to find pachinko distribution","Fatal Error" );
            Application.Exit();
        }

        // Constrain the pachinko add to less than limit if flag is set
        if( diff > 0 )
          slave[i1]++;
        else
          slave[i1]--;

        // Reset localSum, and recompute the sum
        localSum = 0;
        for(i = 0; i < numCols; i++ )
          localSum += slave[i];
        
        diff = target - localSum;
        loopCount++;
      }     // End while

      if( loopCount >= 10000 )
          form.writeToStatusBox( "Standard pachinko failed to resolve difference in 10000 iterations" );
      return diff;
    }     // End method pachinko()

    /*****************************************************************************/

    /* method pachinkoNeg() */
    /// <summary>
    /// Method to perform +1 distribution scheme using a random number and
    /// cumulative distribution to assign values to elements in an array.  This
    /// routine is special for negative emp changes (sitespec) and uses base emp
    /// by sector as a constraint.

    /// </summary>
    /// <param name="baseData">Base data array as constraint for negative values
    /// </param>
    /// <param name="numCols">Number of categories to allocate</param>
    /// <param name="slave">Target area array to receive distribution</param>
    /// <param name="target">Control total of target assignment</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 08/28/97   tb    Initial coding
    *                 08/06/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int pachinkoNeg( BaseForm form, int target, int[] slave,int[] baseData, int numCols )
    {
      int i1,i;
      int loopCount;
      double where;
      int localSum;
      int diff;
      // Computed distribution, max 20 elements
      double[] localDist = new double[form.MAX_TRANS];
      // Cumulative probability
      double[] cumProb = new double[form.MAX_TRANS];
      Random rand = new Random(0);
      // -------------------------------------------------------------------
      // Keep doing this until the target pop is met
      localSum = getArraySum( slave, numCols );
      diff = target - localSum;
      for( loopCount = 0; loopCount < 10000 && diff != 0; loopCount++  )
      {
        getArrayStats( slave, localSum, localDist, cumProb, numCols );
        
        /* Get the random number between 1 and 10000 and convert to xx.xx decimal*/
        where = ( rand.NextDouble() * 100 );
        i1 = 9999;

        // Look for the index of the cumProb <= the random number
        for(i = 0; i < numCols; i++ )
        {
          if( where <= cumProb[i] )     /* If the the random number is less than or equal to this cumProb */
          {
            i1 = i;     // Save the index in the cumProb
            break;
          }   // end if
        }  //end for i

        if( i1 == 9999 )
        {
          MessageBox.Show( "FATAL ERROR - failed to find pachinko distribution" );
          Application.Exit();
        }   // end if

        // Constrain the pachinko change to less than limit
        if( diff > 0 )
          slave[i1]++;
        else if( Math.Abs( slave[i1] ) < baseData[i1] )
          slave[i1]--;
        localSum = getArraySum( slave, numCols );
        diff = target - localSum;
      }     // End for

      if( loopCount == 10000 )
          MessageBox.Show( Environment.NewLine + "pachinkoNeg() failed to resolve difference in 10000 iterations!" );
      return diff;
    }     // End method pachinkoNeg()

    /*****************************************************************************/

    /* method quickSort() */
    /// <summary>
    /// Method to sort a two-dimensional array in ascending order.  Average 
    /// complexity is O(nlogn).  
    /// </summary>
    /// <param name="a">Array to be sorted ascendingly</param>
    /// <param name="hi">Index of the end of the array or partition to be sorted
    /// </param>
    /// <param name="lo">Index of the beginning of the array or partition to be
    ///  sorted</param>
    	
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/11/03   df    Initial coding
    * --------------------------------------------------------------------------
    */
    public static void quickSort( int[,] a, int lo, int hi )
    {
      int i = lo, j = hi, temp1, temp2;
      int x = a[( lo + hi ) / 2, 1];

      do
      {     
        while( a[i,1] < x )
          i++;
        while( a[j,1] > x )
          j--;
        if( i <= j )
        {
          temp1 = a[i,0];
          temp2 = a[i,1];
          a[i,0]=a[j,0];
          a[i,1]=a[j,1];
          a[j,0]=temp1;
          a[j,1]=temp2;
          i++;
          j--;
        }   // end if
      } while( i <= j );

      // Recursion
      if( lo < j )
        quickSort( a, lo, j );
      if( i < hi )
        quickSort( a, i, hi );
    }     // End method quickSort()

    /*****************************************************************************/

    /* method roundIt() */
    /// <summary>
    /// Method to perform rounding with upper bound.
    /// </summary>
    /// <param name="counter">Number of cells to process</param>
    /// <param name="zsw">Switch determining whether or not to check for > 0
    /// </param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 02/17/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int roundIt( int[] local, int[] zcap, int target, int counter, int zsw )
    {
      int i;
      int summer = 0, iterCount = 0;
      int[] temp = new int[counter];
      int diff;
    
      // Sum local data elements
      for( i = 0; i < counter; i++ )
      {
        summer += local[i];
        temp[i] = local[i];
      }

      // + - rounding
      
       while( summer != target && iterCount < 100000 )
      {
        for( i = 0; i < counter; i++ )
        {
          // Only process data with nonzero values
          if( local[i] > 0 || zsw == 1 )
          {
            if( local[i] > 0 && summer > target )
            {
              local[i]--;
              temp[i]--;
              summer--;
            }   // end if
            else if( local[i] < zcap[i] )     /* Don't adjust more than upperbound */
            {
              local[i]++;
              temp[i]++;
              summer++;
            }   // end else if

            if( summer == target )
              break;
          }   // if
        
          //Not greater than zero but cap exists - use this cell.  
          //This should work small cells with 0 allocation, but upper bound is > 0 
          else if( zcap[i] > 0 && summer < target )
          {
            local[i]++;
            temp[i]++;
            summer++;
          }    // end else  
        }     // End for i
        
        iterCount++;
      }     // End while
        
      diff = target - summer;
      return diff;
    }     // End method roundIt()

    /*****************************************************************************/

    /* method roundItLowerLimit() */
    /// <summary>
    /// Method to factor input array to target total and use +/- rounding.  This
    /// roundIt uses lower limits and override flags.  It will make adjustment if
    /// lower constraint exists and it is not overriden - used mainly for HHP
    /// with HH as lower limit.
    /// </summary>
    /// <param name="counter">Number of cells to process</param>
    /// <param name="llimit">Minimum constraint</param>
    /// <param name="local">Source data array</param>
    /// <param name="oFlag">Override flags</param>
    /// <param name="target">Target value</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 02/17/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int roundItLowerLimit( int[] local, bool[] oFlag,int[] llimit, int target,int counter )
    {
      int iterCount = 0, sum = 0;
      int i;
      // --------------------------------------------------------------------
      // Sum local data elements
      for(i = 0; i < counter; i++ )
        sum += local[i];

      // +/- rounding
      while( sum != target && iterCount < 100000 )
      {
        for(i = 0; i < counter; i++ )
        {
          // Only process data with nonzero values and no overrides.
          if( !oFlag[i] && local[i] > 0 )
          {
            if( sum > target )
            {
              if( local[i] > llimit[i] )
              {
                local[i]--;
                sum--;
              }   // end if
            }   // end if
            else  
            {
              local[i]++;
              sum++;
            }   // end else
            if( sum == target )
                break;
          }   // end if
        }     // End for i
        iterCount++;
      }     // End while
      return ( target - sum );
    }     // End method roundItLowerLimit()

    /*****************************************************************************/

    /* method roundItNeg() */
    /// <summary>
    /// Method to factor input array to target total and use +/- rounding.  This
    /// roundit is used for rounding negative changes and override flags.  It
    /// makes adjustment if > not overridden and > base year.

    /// </summary>
    /// <param name="local">Source data array</param>
    /// <param name="baseData">Base year value</param>
    /// <param name="counter">Number of cells to process</param>
    /// <param name="oFlag">Override flags</param>
    /// <param name="target">Target value</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 09/01/98   tb    Initial coding
    *                 08/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int roundItNeg( int[] local, int[] baseData, int target, int counter )
    {
      int sum = 0, iterCount = 0;
      int i;
      // ---------------------------------------------------------------------
      // Sum local data elements
      for(i = 0; i < counter; i++ )
        sum += local[i];

      // +/- rounding
      while( sum != target && iterCount < 100000 )
      {
        for(i = 0; i < counter; i++ )
        {
          if (local[i] == 0 && baseData[i] == 0)
            continue;
          if( sum > target )
          {
            if( Math.Abs( local[i] ) < baseData[i] )
            {
              local[i]--;
              sum--;
            }   // end if
          }   // end if
          else  
          {
            local[i]++;
            sum++;
          }   // end else

          if( sum == target )
            break;
        }     // End for
        iterCount++;
      }     // End while
      return ( target - sum );
    }     // End method roundItNeg()

    /*****************************************************************************/

    /* method roundItNoLimit() */
    /// <summary>
    /// Method to factor input array to target total and use +/- rounding.  This 
    /// roundIt uses no lower or upper limit.
    /// </summary>
    /// <param name="counter">Number of cells to process</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 02/17/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int roundItNoLimit( int[] local, int target, int counter )
    {
      int sum = 0, iterCount;
      int i;
      // Sum local data elements
      for(i = 0; i < counter; i++ )
        sum += local[i];

      // +/- rounding
      iterCount = 0;
      while( sum != target && iterCount < 100000 )
      {
        for(i = 0; i < counter; i++ )
        {
          // Only process data with non-zero values
          if( local[i] > 0 )
          {
            if( sum > target )
            {
              local[i]--;
              sum--;
            }   // end if
            else
            {
              local[i]++;
              sum++;
            }   // end else
            if( sum == target )
              break;
          }   // end if
        }     // End for
    
        iterCount++;
      }     // End while

      return ( target - sum );
    }     // End method roundItNoLimit()

    /*****************************************************************************/

    /* method roundItUpperLimit() */
    /// <summary>
    /// Method to perform rounding for adjusted +/- rounding.  Factor 
    /// input array to target total and use +/- rounding.  Uses upper bound and 
    /// override flag in controlling.  Makes adjustment only if not greater than 
    /// upper constraint and not overriden, otherwise skip.
    /// </summary>
    /// <param name="counter">Number of cells to process</param>
    /// <param name="local">Data array</param>
    /// <param name="oFlag">Override flag</param>
    /// <param name="target">Target sum</param>
    /// <param name="ulimit">Max constraint</param>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 02/17/97   tb    Initial coding
    *                 07/25/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int roundItUpperLimit( int[] local, bool[] oFlag, int[] ulimit, int target, int counter )
    {
      int summer = 0, iterCount;
      bool[] lt1 = new bool[counter];
      int[] lt = new int[counter];
      int[] lc = new int[counter];
      int diff,i;
      // -------------------------------------------------------------------  
      // Sum local data elements
     
      for(i = 0; i < counter; i++ )
      {
        summer += local[i];
        lt[i] = local[i];
        lt1[i] = oFlag[i];
        lc[i] = ulimit[i];
      }   // end for i

      // + - rounding
      iterCount = 0;
      while( summer != target && iterCount < 1000000 )
      {
        for(i = 0; i < counter; i++ )
        {
          // Only process data with nonzero values and not overriden
          if( !oFlag[i]  )     /* Don't adjust more than capacity */
          {
            if( summer > target )
            {
              if (local[i] > 0)
              {
                local[i]--;
                summer--;
              }
            }   // end if
            else if( local[i] < ulimit[i] )
            {
              local[i]++;
              summer++;
            }   // end else if

            if( summer == target )
              break;
          }   // end if
        }     // End for
        
        iterCount++;
      }     // End while

      diff = target - summer;
      return diff;
    }     // End method roundItUpperLimit()

    /*****************************************************************************/

    /* method specialPachinkoEmp() */
    /// <summary>
    /// Method to perform a +1 distribution scheme using a random number and
    /// cumulative distribution to assign values to elements in an array.  This
    /// is a special pachinko for LUZ decrements.
    /// </summary>
    /// <param name="colMarginal">Income group LUZ (column) change</param>
    /// <param name="numElements">Number of MGRAs</param>
    /// <param name="rowMarginals">MGRA HH changes</param>
    /// <param name="slave">Target area array to receive distribution</param>
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/20/98   tb    Initial coding
    *                 09/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int specialPachinkoEmp( int[] slave, int[] control,int[] colMarginals, ref int luzMarginal, int numElements )
    {
      Boolean adjColMar,lookingfori1;
      Boolean [] usei1 = new Boolean[numElements];
      int loopCount, i1, j;
      double where;
      int iwhere;
      int localSum,innercount;
      // Computed distribution, max MGRAs in LUZ.
      double[] localDist = new double[numElements];
      // Cumulative probability
      double[] cumProb = new double[numElements];
      Random rand = new Random(0);
      // -------------------------------------------------------------------
      // Reset the computed arrays
      loopCount = 0;
      localSum = getArraySum( control, numElements );
      getArrayStats( control, localSum, localDist, cumProb, numElements );        
      // Keep doing this until the target pop is met
      for (j = 0; j < numElements; ++j)
      {
        usei1[j] = true;
        if (colMarginals[j] == 0)
          usei1[j] = false;
        if ((Math.Abs(slave[j]) == control[j] && colMarginals[j] < 0))
            usei1[j] = false;
      }   // end for 
      i1 = 9999;
      while( luzMarginal != 0 && loopCount < 1000000)
      {
        lookingfori1 = true;
        while (lookingfori1)
        {
          /* Get the random number between 0 and 1 and convert to xx.xx decimal. */
          where = (rand.NextDouble() * 100);
          iwhere = ((int)where) % 20;
          if (usei1[iwhere])
          {
            i1 = iwhere;     // Save the index in the cumProb
            lookingfori1 = false;
          }  // end if
        }// end while
               
        adjColMar = true;
        while (adjColMar)
        {
          if (colMarginals[i1] > 0)
          {
            slave[i1]++;
            if (luzMarginal > 0)
              --luzMarginal;
            else
              ++luzMarginal;
            colMarginals[i1]--;
            adjColMar = false;
          }  // end if
          else if ((colMarginals[i1] < 0) && (control[i1] > 0) && (Math.Abs(slave[i1]) < control[i1]))
          {         
              --slave[i1];
              if (luzMarginal > 0)
                --luzMarginal;
              else
                ++luzMarginal;
              ++colMarginals[i1];
              if (slave[i1] == control[i1])
                usei1[i1] = false;

              if (colMarginals[i1] == 0)
              usei1[i1] = false;
            adjColMar = false;
          }
          else if (colMarginals[i1] == 0)  // if the col marginal = 0, find one that isn't and adjust that - this
          // compensates for screwy distributions and forces something to get done on every rand pass
          {
            innercount = 0;
            while (colMarginals[i1] == 0 && innercount < 1000)
            {
              ++i1;
              if (i1 >= numElements)
                i1 = 0;
              ++innercount;
            } // end while

          }  // end else if
          else if (Math.Abs(slave[i1]) >= control[i1])
          {
            innercount = 0;
            while (Math.Abs(slave[i1]) >= control[i1] && innercount < 1000)
            {
              ++i1;
              if (i1 >= numElements)
                i1 = 0;
              ++innercount;
            } // end while
          }
        }   // end while adjColMar
        for (j = 0; j < numElements; ++j)
        {
          if (colMarginals[j] == 0 || (Math.Abs(slave[j]) == control[j] && colMarginals[j] < 0))
            usei1[j] = false;
        }   // end for
        loopCount++;
      }     // End while
     
      return luzMarginal;
    }     // End method specialPachinkoEmp

    //***************************************************************************

    /* method specialPachinko1() */
    /// <summary>
    /// Method to perform a +1 distribution scheme using a random number and
    /// cumulative distribution to assign values to elements in an array.  This
    /// is a special pachinko for LUZ decrements.
    /// </summary>
    /// <param name="colMarginal">Income group LUZ (column) change</param>
    /// <param name="numElements">Number of MGRAs</param>
    /// <param name="rowMarginals">MGRA HH changes</param>
    /// <param name="slave">Target area array to receive distribution</param>
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/20/98   tb    Initial coding
    *                 09/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int specialPachinko1( int[] slave, int[] rowMarginals, ref int colMarginal, int numElements )
    {
      int loopCount, i1,i ;
      double where;
      int localSum;
      // Computed distribution, max MGRAs in LUZ.
      double[] localDist = new double[numElements];
      // Cumulative probability
      double[] cumProb = new double[numElements];
      Random rand = new Random(0);
      // -------------------------------------------------------------------
      // Reset the computed arrays
      loopCount = 0;
              
      // Keep doing this until the target pop is met
      while( colMarginal != 0 && loopCount < 100000 )
      {
        localSum = getArraySum( slave, numElements );
        getArrayStats( slave, localSum, localDist, cumProb, numElements );
        /* Get the random number between 0 and 1 and convert to xx.xx decimal. */
        where = rand.NextDouble() * 100 ;
        

        i1 = 9999;
        // Look for the index of the cumProb <= the random number
        for(i = 0; i < numElements; i++ )
        {
          if( where <= cumProb[i] )
          {
            i1 = i;     // Save the index in the cumProb
            break;
          }
        }   // end for i

        if( i1 == 9999 )
          return i1;

        if( slave[i1] > 0 )
        {
          slave[i1]--;
          colMarginal++;
          rowMarginals[i1]++;
        }   // end if
        loopCount++;
      }     // End while

      return colMarginal;
    }     // End method specialPachinko1

    /*****************************************************************************/

    /* method specialPachinko2() */
    /// <summary>
    /// Method to perform a +1 distribution scheme using a random number and 
    /// cumulative distribution to assign values to elements in an array.  This 
    /// is a special pachinko for MGRA decrements.

    /// </summary>
    /// <param name="colMarginal">Income group LUZ (column) change</param>
    /// <param name="numElements">Number of income groups</param>
    /// <param name="rowMarginal">MGRA HH changes</param>
    /// <param name="slave">Target area array to receive distribution</param>
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/20/98   tb    Initial coding
    *                 09/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int specialPachinko2( int[] slave, ref int rowMarginal,int[] colMarginals, int numElements )
    { 
      int i1,i;
      int loopCount = 0;
      double where;
      int localSum;
      // Max 13 also used for income with 8 cats computed distribution
      double[] localDist = new double[numElements];
      // Cumulative probability
      double[] cumProb = new double[numElements];

      Random rand = new Random(0);
      // ----------------------------------------------------------------------
          // Keep doing this until the target pop is met
      while( rowMarginal != 0 && loopCount < 100000 )
      {
        localSum = getArraySum( slave, numElements );
        getArrayStats( slave, localSum, localDist, cumProb, numElements );
        
        where = ( rand.NextDouble() * 100 );
        if( where == 0 )
          continue;
        i1 = 9999;

        // Look for the index of the cumProb <= the random number
        for(i = 0; i < numElements; i++ )
        {
          if( where <= cumProb[i] )
          {
            i1 = i;     // Save the index in the cumProb
            break;
          }   // end if
        }   // end for i

        if( i1 == 9999 )
        {
          MessageBox.Show( "FATAL ERROR - failed to find pachinko distribution" );
          return i1;;
        }   // end if
        if( slave[i1] > 0 )
        {
          slave[i1]--;
          rowMarginal++;
          colMarginals[i1]++;
        }   // end if
        loopCount++;
      }     // End while
      return rowMarginal;
    }     // End procedure specialPachinko2

    /*****************************************************************************/

    /* method specialPachinko3() */
    /// <summary>
    /// Method to perform a +1 distribution scheme using a random number and 
    /// cumulative distribution to assign values to elements in an array.  This 
    /// is a special pachinko for positive MGRA and LUZ changes.

    /// </summary>
    /// <param name="colMarginal">Income group LUZ (column) change</param>
    /// <param name="numElements">Number of income groups</param>
    /// <param name="rowMarginal">MGRA HH changes</param>
    /// <param name="slave">Target area array to receive distribution</param>
      
    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 07/20/98   tb    Initial coding
    *                 09/04/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static int specialPachinko3( int[] slave, ref int rowMarginal,int[] colMarginals, int numElements )
    {
      int i1,i;
      int loopCount = 0;
      double where;
      int localSum;

      int[] localData = new int[numElements];     
      // Computed distribution, max MGRAs in LUZ
      double[] localDist = new double[numElements];
      // Cumulative probability
      double[] cumProb = new double[numElements];      
      Random rand = new Random(0);

      // Store slave array for debug printing
      for(i = 0; i < numElements; i++ )
        localData[i] = slave[i];

      // Keep doing this until the target is met
      while( rowMarginal != 0 && loopCount < 100000 )
      {
        localSum = getArraySum( colMarginals, numElements );
        getArrayStats( colMarginals, localSum, localDist, cumProb,numElements );
        where = ( rand.NextDouble() * 100 );
        if( where == 0 )
          continue;
        i1 = 9999;
        // Look for the index of the cumProb <= the random number
        for(i = 0; i < numElements; i++ )
        {
          if( where <= cumProb[i] )
          {
              i1 = i;     // Save the index in the cumProb
              break;
          }   // end if
        }   // end for i
        if( i1 == 9999 )
          return i1;

        slave[i1]++;
        rowMarginal--;
        colMarginals[i1]--;
        loopCount++;
      }     // End while

      return rowMarginal;
    }     // End method specialPachinko3

    /*****************************************************************************/

    /* method storeZB() */
    /// <summary>
    /// Method to store luzBase query results in z array, and also compute some 
    /// LUZ data for forecast and set some flags.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/23/03   df    C# revision
    *                 08/30/05   tb    sr11 changes
    * --------------------------------------------------------------------------
    */
    public static void storeZB( bool minSwitch, int i, int[] zbi,double minParam, Master reg, Master[] z ,int numEmpSectors)
    {
      int j = 0;
      int iter = 0;
      //----------------------------------------------------------------------
      // Is actual LUZ id (NUM_LUZS-1) in cordon area (have to add 1 to index)
      if( i + 1 >= 193 && i+1 <= 215 )
        z[i].cordon = false;     // Set cordon flag to outside area
      else
        z[i].cordon = true;     // Inside cordon area

      z[i].baseData.p.pop = zbi[iter++];
      z[i].baseData.p.hhp = zbi[iter++];
      z[i].baseData.p.er = zbi[iter++];
      z[i].baseData.p.gq = zbi[iter++];
      z[i].baseData.p.gqCiv = zbi[iter++];
      z[i].baseData.p.gqMil = zbi[iter++];
      z[i].baseData.p.gqCivCol = zbi[iter++];
      z[i].baseData.p.gqCivOth = zbi[iter++];

      z[i].baseData.hs.total = zbi[iter++];
      z[i].baseData.hs.sf = zbi[iter++];
      z[i].baseData.hs.mf = zbi[iter++];
      z[i].baseData.hs.mh = zbi[iter++];

      z[i].baseData.hh.total = zbi[iter++];
      z[i].baseData.hh.sf = zbi[iter++];
      z[i].baseData.hh.mf = zbi[iter++];
      z[i].baseData.hh.mh = zbi[iter++];

      z[i].baseData.i.median = zbi[iter++];

      for( j = 0; j < 10; j++ )
        z[i].baseData.i.hh[j] = zbi[iter++];

          // Employment categories
      z[i].baseData.e.total = zbi[iter++];
      z[i].baseData.e.adj = zbi[iter];   // dont increment her because adj and emp are equal
      z[i].baseData.e.civ = zbi[iter++];
      z[i].baseData.e.mil = zbi[iter++];
                     
      for( j = 0; j < numEmpSectors; j++ )
        z[i].baseData.e.sectors[j] = zbi[iter++];
          
      /* If minimum constraints are imposed, compute constraint as 1.xxx * base employment */
      // Compute minimum constraint
      if( minSwitch )
        z[i].minEmp = ( int )( ( minParam / 100 ) * ( double )z[i].baseData.e.civ ); 

      // Save these in the region totals
    
      iter = 0;
      reg.baseData.p.pop += zbi[iter++];
      reg.baseData.p.hhp += zbi[iter++];
      reg.baseData.p.er += zbi[iter++];
      reg.baseData.p.gq += zbi[iter++];
      reg.baseData.p.gqCiv += zbi[iter++];
      reg.baseData.p.gqMil += zbi[iter++];
      reg.baseData.p.gqCivCol += zbi[iter++];
      reg.baseData.p.gqCivOth += zbi[iter++];

      reg.baseData.hs.total += zbi[iter++];
      reg.baseData.hs.sf += zbi[iter++];
      reg.baseData.hs.mf += zbi[iter++];
      reg.baseData.hs.mh += zbi[iter++];
          
      reg.baseData.hh.total += zbi[iter++];
      reg.baseData.hh.sf += zbi[iter++];
      reg.baseData.hh.mf += zbi[iter++];
      reg.baseData.hh.mh += zbi[iter++];

      for( j = 0; j < 10 ; j++ )   //income groups
          reg.baseData.i.hh[j] = zbi[iter++];

      reg.baseData.e.total += zbi[iter++];
      reg.baseData.e.civ += zbi[iter];   // dont increment
      reg.baseData.e.adj += zbi[iter++];
      reg.baseData.e.mil += zbi[iter++];

      for( j = 0; j < numEmpSectors; j++ )
          reg.baseData.e.sectors[j] += zbi[iter++];
      
    }     // End method storeZB()

    /*****************************************************************************/

    /* method storeZH() */
    /// <summary>
    /// Method to store LUZ history query results into zh array.
    /// </summary>

    /* Revision History
    * 
    * STR             Date       By    Description
    * --------------------------------------------------------------------------
    *                 05/22/97   tb    Initial coding
    *                 07/17/03   df    C# revision
    * --------------------------------------------------------------------------
    */
    public static void storeZH( Master reg, Master z, int[] e, int[] s, int[] m, int[] h )
    {
      z.histEmp.L5 = e[0];
      z.histEmp.L0 = e[1];
      z.histSF.L5 = s[0];
      z.histSF.L0 = s[1];
      z.histMF.L5 = m[0];
      z.histMF.L0 = m[1];
      z.histHH.L5 = h[0];
      z.histHH.L0 = h[1];

      // Build the regional totals on the fly   
      reg.histEmp.L5 += e[0];
      reg.histEmp.L0 += e[1];
      reg.histSF.L5 += s[0];
      reg.histSF.L0 += s[1];
      reg.histMF.L5 += m[0];
      reg.histMF.L0 += m[1];
      reg.histHH.L5 += h[0];
      reg.histHH.L0 += h[1];
    }     // End method storeZH()

    // *******************************************************************************

  }     // End class UDMUtils
}     // End namespace udm.utils