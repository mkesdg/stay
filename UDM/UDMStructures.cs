/* Filename:    UDMStructures.cs
 * Program:     UDM
 * Version:     7.0 sr13
 * Programmers: Terry Beckhelm & Daniel Flyte (C# revision)
 * Description: This class set provides a comprehensive collection of data
 *              structures that are used in UDM.
 */
 

using System;

namespace Sandag.TechSvcs.RegionalModels
{
    public class GD
    {
        public const int NUM_EMP_LAND = 7;
        public const int NUM_MF_LAND = 4;
        public const int NUM_SF_LAND = 5;
        public const int NUM_EMP_SECTORS = 20;
        public const int NUM_HH_INCOME = 10;
    }

    /// Acres data structure
    public class Acres
    {
        public double[] ae;
        public double[] asf;        /// Acres by SF LU cat
        public double[] amf;        /// Acres by MF LU cat
        public double totalEmpAcres;/// Total emp acres
        public double totalSFAcres; /// Total SF acres
        public double totalMFAcres; /// Total MF acres

        public Acres()
        {
            ae = new double[GD.NUM_EMP_LAND];
            asf = new double[GD.NUM_SF_LAND];
            amf = new double[GD.NUM_MF_LAND];
        }
    }

    /// Capacity data structure
    public class Cap
    {          
        public int[] e;                     /// 6 UDM employment categories, starts at 1; 0 is a dummy for indexing.
        public int[] sf;/// 4 UDM SF categories; 0 is a dummy
        public int[] mf;/// 3 UDM MF categories; 0 is a dummy
        public int totalEmp;/// Total emp capacity
        public int totalSF; /// Total SF capacity
        public int totalMF;/// Total MF capacity
        public int totalmh;/// Total mh capacity
        public double[] pe;/// Percent capacity used by 6 emp land uses
        public double[] pSF; /// Percent capacity used by 4 SF land uses
        public double[] pMF;/// Percent capacity used by 3 MF land uses         
        public double pTotalEmp; /// Percent capacity used by total emp         
        public double pTotalSF; /// Percent capacity used by SF          
        public double pTotalMF;/// Percent capacity used by MF          
        public double pTotalmh;/// Percent capacity used by mh         
        public Acres ac; /// Acres object

        public Cap()
        {
            e = new int[GD.NUM_EMP_LAND];
            sf = new int[GD.NUM_SF_LAND];
            mf = new int[GD.NUM_MF_LAND];
            pe = new double[GD.NUM_EMP_LAND];
            pSF = new double[GD.NUM_SF_LAND];
            pMF = new double[GD.NUM_MF_LAND];
            ac = new Acres();
        }
    }
    
    /// Employment distribution by land use
    public class EmpLU
    {
        private const int NUM_EMP_SECTORS = 20;       
        public int plu;/// Planned land use - lookup key        
        public double[] dist;/// Emp distribution for this LU

        public EmpLU()
        {
            dist = new double[NUM_EMP_SECTORS + 1];
        }
    }
    /// Employment categories data structure
    public class EmpCats
    {
        public int total;/// Total employment         
        public int adj; /// Adjusted for sitespec          
        public int civ;/// Total civ emp          
        public int mil;/// Total mil emp          
        public int cnws;/// Civilian nonag wage and salaried emp          
        public int[] sectors;/// Employment sectors          
        public int[] sectorsAdj;/// Sectors adjusted for sitespec          
        public int[] empLand;/// Employment by land use category

        public EmpCats()
        {
            sectors = new int[GD.NUM_EMP_SECTORS];
            sectorsAdj = new int[GD.NUM_EMP_SECTORS];
            empLand = new int[GD.NUM_EMP_LAND];
        }
    }

    /// Employment overrides data structure
    public class EO
    {
        public int adj;/// Civilian employment total          
        public int[] elu;/// Six land use categories, 0 is dummy

        public EO()
        {
            elu = new int[GD.NUM_EMP_LAND];
        }
    }

    /// Employed residents data structure
    public class ERA
    { 
        public int total;/// Total employed residents = empCiv          
        public int sfAuto;/// Employed residents in SF housing using cars         
        public int sfTransit; /// Employed residents in SF housing using transit         
        public int mfAuto; /// Employed residents in MF housing using cars         
        public int mfTransit; /// Employed residents in MF housing using transit         
        public int mfAlloc; /// Derived MF from LUZ-LUZ computations (allocations)          
        public int sfAlloc;/// Derived SF from luz-luz computations (allocations)
    }


    /// Global flags structure.  A flag set means there is data in output file.
    public class Flags
    {
        public bool empChange;
        public bool housingChange;          
        public bool dcOut;/// Detailed characteristics outliers          
        public bool dcOvr;/// Detailed characteristics overrides         
        public bool eOut; /// Emp outliers          
        public bool hOut;/// HS outliers        
        public bool eOvr; /// Employment allocation overrides          
        public bool esOvr;/// Employment sector overrides         
        public bool hOvr; /// HS overrides         
        public bool rOvr;/// Rates overrides
        public bool except;
        public bool minCons;
        public bool redisEmp;
        public bool redisHS;
        public bool schPart;         
        public bool zit;/// Rebuild luz intermediate table.
    }
    /// Housing and households data structure
    public class HCats
    {
        public int total;         
        public int sf; /// SF from all sources          
        public int mf;/// MF from all sources         
        public int mh; /// mh from all sources         
        public int sfAdj; /// SF allocation adjusted for sitespec and loses          
        public int mfAdj;/// MF allocation adjusted for sitespec and loses          
        public int mhAdj;/// mh allocation adjusted for sitespec and loses         
        public int[] mfLand; /// 3 MF land cats, 0 is dummy         
        public int[] sfLand; /// 4 SF land cats, 0 is dummy

        public HCats()
        {
          mfLand = new int[GD.NUM_MF_LAND];
          sfLand = new int[GD.NUM_SF_LAND];
        }
    }

    /// History data structure
    public class History
    {
        public int L5;/// 5 year level         
        public int L0; /// Base year level          
        public int c5;/// 5 year change          
        public double r5;/// 5 year change ratio         
        public double pct5; /// 5 year percent change
    }

    // LUZ HS overrides data structure
    public class HSO
    {
        public int attrSF;         // Attractor SF
        public int attrMF;         // Attractor MF
        public int sf;             // SF stock
        public int mf;             // MF stock
        public int mh;             // mh stock
        public int[] sfLU;         // SF by land use (0 is dummy)
        public int totalSFLU;      // Total SF land use override
        // There are no land use overrides for md.

        public HSO()
        {
          sfLU = new int[GD.NUM_SF_LAND];
        }
    }
  
    // Income data structure
    public class Income
    {
        
        public int[] hh;           // Number of households in each income group
        public double[] dist;      // % distribution
        public int median;         // Median income

        public Income()
        {
          hh = new int[GD.NUM_HH_INCOME];
          dist = new double[GD.NUM_HH_INCOME];
        }
    }

    // Outliers data strucure
    public class Outliers
    {
        public double r5;
        public int outCode;
        public int inc5;
        public int diff5;
    }

    // Population, gq and er data structure
    public class PCats
    {
        public int pop;            // Total pop
        public int hhp;            // Household pop
        public int er;             // Employed residents
        public int gq;             // Total group quarters
        public int gqCiv;          // Civilian group quarters
        public int gqCivAdj;       // Adjusted for sitespec
        public int gqCivExtra;     // Base + site spec used in raking procedures
        public int gqMil;          // Military group quarters
        public int gqMilAdj;       // Adjusted for sitespec
        public int gqCivCol;       // civ college
        public int gqCivOth;       // civ other
       
    }

    
    // Pct change
    public class PctDef
    {
        public double civ;      // Civ employment
        public double total;    // Total employment 
        public double sf;       // SF units
        public double mf;       // MF units
        public double hh;       // hh
        public double hhp;      // hhp
        public double gq;       // Total gq
        public double pop;      // Total pop
        public double erHH;     // Employed residents
        public double hhs;      // Household size
        public double v;        // Vacancy rate and by structure type
        public double vSF;
        public double vMF;
        public double vmh;
        public double medianIncome;
        public double[] sectors;       // Employment sectors
        public double[] incomeHH;      // Income distribution

        public PctDef()
        {
          sectors = new double[GD.NUM_EMP_SECTORS];
          incomeHH = new double[GD.NUM_HH_INCOME];
        }
    }

    // Plus minus data structure
    public class PlusMinusValues
    {
        public int control;
        public int summ;
        public int sumAbs;
        public double pAdj;
        public double nAdj;
        public bool adjFlag;
    }

    public class Rates
    {
        public double v;         // Total vacancy rate
        public double vSF;       // Vacancy rate - SF
        public double vMF;       // Vacancy rate - MF
        public double vmh;       // Vacancy rate - mh
        public double erHH;      // Employed residents/hh
        public double hhs;       // HH size
        public bool regOvr;      // Regional overrides used flag
    }

    // LUZ rates overrides structure
    public class RO
    {
        public double vSF;           // Vacancy rate SF
        public double vMF;           // Vacancy rate MF
        public double vmh;           // Eacancy rate mh
        public double erHH;          // Employed residents/hh
        public double hhs;           // hhs
        public int medianIncome;     // Median income in income equation
        public double asd;           /* Adjusted standard deviation in income equation */
        public double nla;           /* Nonlinear adjustment - calibration exponent equation */
        public int incomeSwitch;     /* Income equation switch.  1 = use reg dist; 2 = luz base year dist. */
    }

    // Sitespec data structure
    public class SS
    {
        public int civ;        // Civ emp
        public int sf;         // SF units
        public int mf;         // MF units
        public int mh;         // mh units
        public int gqCiv;      // Civ gq
        public int gqMil;      // Mil gq
        public int[] sectors;  // Employment by sector
        public Acres ac;       // Developable acres structure

        public SS()
        {
            sectors = new int[GD.NUM_EMP_SECTORS];
            ac = new Acres();
        }
    }

    public class TCap
    {
        public int luz;
        public int mgra;
        public int LCKey;
        public int lu;
        public int plu;
        public int devCode;
        public int udmEmpLU;
        public int udmMFLU;
        public int udmSFLU;
        public int civ;
        public int capCiv;
        public int sf;
        public int mf;
        public int mh;
        public int capSF;
        public int capMF;
        public int capmh;
        public int oldCapCiv;
        public int oldCapSF;
        public int oldCapMF;
        public int oldCapmh;
        public int chgSF;
        public int chgMF;
        public int chgmh;
        public int chgCiv;
        public double pCap_hs;
        public double pCap_emp;
        public double oldpCap_hs;
        public double oldpCap_emp;
        public double acres;
        public bool done;
        public bool goodLand;
        public int gq_civ;
    }

    public class TCapD
    {
        public int luz;
        public int mgra;
        public int LCKey;
        public int civ;
        public int devCode;
        public int capCiv;
        public int chgCiv;
        public int mh;
        public int mf;
        public int sf;
        public int capMh;
        public int capMf;
        public int capSf;
        public int chgMh;
        public int chgMf;
        public int chgSf;
        public double pCap_hs;
        public double pCap_emp;
        public bool done;
    }


    // Travel time curve parameters
    public class TTP
    {
        public double med;
        public double asd;
        public double nla;
    }

    public class BaseData
    {
        public PCats p;            // Population structure, pop, hhp etc
        public Income i;
        public EmpCats e;          // Employment
        public HCats hs;           // Housing stock
        public HCats hh;           // Households
        public Rates r;            // Vacancy rates, er/hh and pph
        public ERA er;             // Employed residents structure
       
        public BaseData()
        {
            p = new PCats();
            i = new Income();
            e = new EmpCats();
            hs = new HCats();
            hh = new HCats();
            r = new Rates();
            er = new ERA();
        }
    }

    public class ForecastData
    {
        // Income parameters and adjustments
        public double asd;
        public double nla;
        public double[] incomeAdj;

        public double share;
        public double ashare;
        public int eCapShare;        /* Employment fcst based on share of regional capacity */
        public int mfCapShare;       /* MF fcst based on share of regional capacity */
        public int sfCapShare;       // SF fcst based on share of regional capacity

        // Employment equations vars
        public double hhR;       // hh as % of regional total
        public double civR;      // Total base year emp as % of regional total

        // Potential lost mh units
        public int plmh;

        // Various data structures
        public PctDef pct;     // Percent change
        public PCats p;        // Pop structure - levels
        public PCats pi;       // Pop structure - increment
        public EmpCats e;      // luz employment structure levels
        public EmpCats ei;     // luz employment structure increments
        public HCats hs;       // luz housing structure levels - housing stock
        public HCats hh;       // luz households structure levels
        public HCats hsi;      // luz housing structure increment - housing stock
        public HCats hhi;      // luz housing structure increment - households
        public HCats le;       // Units lost due to emp redev  (a negative number)
        public HCats lh;       // Units lost du to HS redev
        public Income i;       // Income data structure - levels
        public Income ii;      // Income data structure - increment
        public Rates r;        // Vacancy rates, er/hh and pph - levels
        public Rates ri;       // Vacancy rates, er/hh and pph - increment
        public ERA eri;        // Employed resident structure - increment

        public ForecastData()
        {
          incomeAdj = new double[GD.NUM_HH_INCOME];
          pct = new PctDef();
          p = new PCats();
          pi = new PCats();
          e = new EmpCats();
          ei = new EmpCats();
          hs = new HCats();
          hh = new HCats();
          hsi = new HCats();
          hhi = new HCats();
          le = new HCats();
          lh = new HCats();
          i = new Income();
          ii = new Income();
          r = new Rates();
          ri = new Rates();
          eri = new ERA();
        }
    }
        
    /// luz data structure, also used for luz aggregation areas and regional totals

    public class Master
    {
        public double homePriceIndex;  // home price index
        public int zone;               // Aggregation area ID
        public int group;              // Aggregation group 0-2
        public int incomeSwitch;       /* Income model equation switch 1 - reg dist; 2 - base year; 3 (default) lognormal rates overrides may overrride default. */
        public int exCap;              // At or over capacity flag
    
        public double baseRatio;       // Base year employment ratio
  
        public bool cordon;            // luz in cordon area
    
        public bool eOvr;              // Employment increment overrides exist
        public bool elOvr;             // Employment land use overrides exist
        public bool esOvr;             // Employment sector overrides
        public bool hOvr;
        public bool iOvr;              // dc income overrides
        public bool erOvr;             // dc er_hh rates overrides
        public bool vacOvr;            // dc vacancy rates overrides flag
        public bool hhsOvr;            // dc hhs overrides
        public bool sfOvr;             // HS SF overrides exist
        public bool mfOvr;             // HS MF overrides exist
        public bool mhOvr;             // HS mh overrides exist
       
        public int minFlag;            // Under minimum employment constraints
        public int minEmp;             // Minimum constraints employment
        public int gain;               // Gain in emp - used for regional total
        public int loss;               // Loss in emp - used for regional total
        public string name;            // Aggregation area name

        public BaseData baseData;      // Base year data structure
        public ForecastData fcst;      // Forecast data structure

        // History data structures
        public History histEmp;        // Employment 
        public History histSF;         // SF 
        public History histMF;         // MF 
        public History histHH;         // hh 
 
        public Outliers eOut;          // Outliers structure 
        public Outliers mfOut;
        public Outliers sfOut;

        public SS site;                // Site spec structure
        public Cap capacity;           // Incremental capacity structure
        public Cap useCap;             // Capacity used structure
        public Cap remCap;             // Remaining capacity structure
        public EO eo;                  // Employment overrides structure
        public HSO ho;                 // Housing overrides structure
        public EmpCats eso;            // Employment sector overrides
        public RO ro;                  // luz rates overrides

        public Master()
        {
            baseData = new BaseData();
            fcst = new ForecastData();
            histEmp = new History();
            histSF = new History();
            histMF = new History();
            histHH = new History();
            eOut = new Outliers();
            mfOut = new Outliers();
            sfOut = new Outliers();
            site = new SS();
            capacity = new Cap();
            useCap = new Cap();
            remCap = new Cap();
            eo = new EO();
            ho = new HSO();
            eso = new EmpCats();
            ro = new RO();
        }
    }
    public class Forecast2
    {
        public EmpCats e;/// Employment levels          
        public EmpCats ei;/// Employment increment          
        public PCats p;/// Pop structure - levels         
        public PCats pi; /// Pop structure - increment         
        public HCats hs; /// HS structure - levels          
        public HCats hsi;/// HS structure - increment         
        public HCats hh; /// HH structure - levels          
        public HCats hhi;/// HH structure - increment          
        public Rates r;/// Vacancy rates          
        public Income i;/// Income structure - levels         
        public Income ii; /// Income structure - increment          
        public PctDef pct;/// Percent change structure        
        public double[] empLU = new double[4];// UDM data for 3 employment by land use

        /// UDM data for 51 land use types
        /// this is the new array order recoded 02/06/06 by tbe
        /// this reflects the column order in mgrabase table for acreage
        public double[] acres;

       /* 0 - dev_ldsf	
        * 1 - dev_sf	
        * 2 - dev_mf	
        * 3 - dev_mh	
        * 4 - dev_oth_res	
        * 5 - dev_ag	
        * 6 - dev_indus	
        * 7 - dev_comm	
        * 8 - dev_office	
        * 9 - dev_schools	
        * 10 - dev_roads	
        * 11 - dev_parks	
        * 12 - dev_mil	
        * 13 - dev_water	
        * 14 - dev_mixed_use	
        * 15 - vac_ldsf	
        * 16 - vac_sf	
        * 17 - vac_mf	
        * 18 - vac_mh	
        * 19 - vac_oth_res	
        * 20 - vac_ag	
        * 21 - vac_indus	
        * 22 - vac_comm	
        * 23 - vac_office	
        * 24 - vac_schools	
        * 25 - vac_future_roads	
        * 26 - vac_mixed_use	
        * 27 - vac_parks	
        * 28 - redev_sf_mf	
        * 29 - redev_sf_emp	
        * 30 - redev_mf_emp	
        * 31 - redev_mh_sf	
        * 32 - redev_mh_mf	
        * 33 - redev_mh_emp	
        * 34 - redev_ag_ldsf	
        * 35 - redev_ag_sf	
        * 36 - redev_ag_mf	
        * 37 - redev_ag_indus	
        * 38 - redev_ag_comm	
        * 39 - redev_ag_office	
        * 40 - redev_ag_schools	
        * 41 - redev_ag_roads	
        * 42 - redev_emp_res	
        * 43 - redev_emp_emp	
        * 44 - infill_sf	
        * 45 - infill_mf	
        * 46 - infill_emp	
        * 47 - acres	
        * 48 - dev	
        * 49 - vac	
        * 50 - unusable	
        */

        public Forecast2()
        {
          e = new EmpCats();
          ei = new EmpCats();
          p = new PCats();
          pi = new PCats();
          hs = new HCats();
          hsi = new HCats();
          hh = new HCats();
          hhi = new HCats();
          r = new Rates();
          i = new Income();
          ii = new Income();
          pct = new PctDef();
          acres = new double[51];
        }
    }
    public class Base2
    {
        public EmpCats e;/// Employment levels          
        public PCats p;/// Pop structure - levels         
        public HCats hs; /// HS structure - levels         
        public HCats hh; /// HH structure - levels          
        public Rates r;/// Vacancy rates                       
        public Income i;/// Income data structure - levels
        public double[] eDist;
        public double[] acres;

        public Base2()
        {
            e = new EmpCats();
            p = new PCats();
            hs = new HCats();
            hh = new HCats();
            r = new Rates();
            i = new Income();
            eDist = new double[21];
            acres = new double[51];
        }
    }
    /// mgraBase variables data structure
    public class MBMaster
    {
        public int mgra;/// MGRA ID          
        public int luz;/// LUZ ID

        public Base2 baseData;
        public Forecast2 fcst;
        public SS site;

        public MBMaster()
        {
            baseData = new Base2();
            fcst = new Forecast2();
            site = new SS();
        }
    }

    /// Regional controls totals data structure
    public class RegionalControls
    {
        public Base2 baseData;
        public Forecast2 fcst;          
        public SS site;/// Variety of sitespec data          
        public Cap capacity;/// Capacity data structure

        public RegionalControls()
        {
            baseData = new Base2();
            fcst = new Forecast2();
            site = new SS();
            capacity = new Cap();
        }
    }

    public class Transactions
    {
        public int mgra;
        public int luz;
        public int site;
        public int LCKey;
        public int lu;
        public int plu;
        public int devCode;
        public int gqCiv;
        public int gqMil;
        public int chgempCiv;
        public int chgempMil;
        public int sf;
        public int mf;
        public int mh;
        public double acres;
        public double pCap_hs;
        public int emp;
        public int civ;
        public int mil;
	    public double pCap_emp;
    }

    public class TableNames
    {
        public string accessWeights;
        public string capacity;
        public string capacity1;
        public string capacity2;
        public string capacity3;
        public string capacity4;
        public string capacityNext;
        public string empDecrements;
        public string empDistByLU;
        public string empIncrements;
        public string fractee;
       
        public string homePrices;
        public string impedAM;
        public string impedPM;
        public string impedTran;
        public string incomeDistAdj;
        public string luzbase;
        public string luzhist;
        public string luztemp;
        public string luzEmpOvr;
        public string luzEmpSectorOvr;
        public string luzEROvr;
        public string luzMFOvr;
        public string luzSFOvr;
        public string luzHHSOvr;
        public string luzIncOvr;
        public string luzIncomeParms;
        public string luzVacOvr;
        public string mgrabase;
        public string mgrabase_adjusted_for_prisons;
        public string MFDecrements;
        public string MFIncrements;
        public string mhDecrements;
        public string mhIncrements;
        public string milSiteSpec;
        public string prison_temp;
        public string regfcst;
        public string SFDecrements;
        public string SFIncrements; 
        public string updateGQC;
        public string xref;
        
    }

}     // End namespace udm.Structures
