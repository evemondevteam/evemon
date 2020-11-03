using System.Collections.ObjectModel;

namespace EVEMon.Common.Constants
{
    /// <summary>
    /// Provides constants about the CCP databases.
    /// </summary>
    public static class DBConstants
    {
        #region Custom Constants

        public const string GeneralCategoryName = "General";
        public const string PropulsionCategoryName = "Propulsion";
        public const string ConsumptionRatePropertyName = "Consumption Rate";
        public const string PackagedVolumePropertyName = "Packaged Volume";

        #endregion


        #region Collections

        // Group of PropertyIDs
        public static ReadOnlyCollection<int> RequiredSkillPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            RequiredSkill1PropertyID, RequiredSkill2PropertyID,
            RequiredSkill3PropertyID, RequiredSkill4PropertyID,
            RequiredSkill5PropertyID, RequiredSkill6PropertyID
        });

        public static ReadOnlyCollection<int> RequiredSkillLevelPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            RequiredSkill1LevelPropertyID, RequiredSkill2LevelPropertyID,
            RequiredSkill3LevelPropertyID, RequiredSkill4LevelPropertyID,
            RequiredSkill5LevelPropertyID, RequiredSkill6LevelPropertyID
        });

        public static ReadOnlyCollection<int> AlwaysVisibleForShipPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            CPUOutputPropertyID, PGOutputPropertyID, UpgradeCapacityPropertyID,
            HiSlotsPropertyID, MedSlotsPropertyID, LowSlotsPropertyID,
            DroneCapacityPropertyID, DroneBandwidthPropertyID, CargoCapacityPropertyID,
            MassPropertyID, VolumePropertyID, CapacitorCapacityPropertyID,
            CapacitorRechargeRatePropertyID, MaxTargetRangePropertyID,
            ScanResolutionPropertyID, SignatureRadiusPropertyID, MaxVelocityPropertyID,
            ShipWarpSpeedPropertyID, StructureHitpointsPropertyID, ShieldHitpointsPropertyID,
            ArmorHitpointsPropertyID, ShieldRechargeRatePropertyID,
            ShieldEMResistancePropertyID, ShieldExplosiveResistancePropertyID,
            ShieldKineticResistancePropertyID, ShieldThermalResistancePropertyID,
            ArmorEMResistancePropertyID, ArmorExplosiveResistancePropertyID,
            ArmorKineticResistancePropertyID, ArmorThermalResistancePropertyID
        });

        public static ReadOnlyCollection<int> HideIfDefaultPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            LauncherSlotsLeftPropertyID, TurretSlotsLeftPropertyID,
            TurretHardPointModifierPropertyID, LauncherHardPointModifierPropertyID,
            HiSlotModifierPropertyID, MedSlotModifierPropertyID,
            LowSlotModifierPropertyID, ScanRadarStrengthPropertyID,
            ScanLadarStrengthPropertyID, ScanMagnetometricStrengthPropertyID,
            ScanGravimetricStrengthPropertyID, HullEMResistancePropertyID,
            HullExplosiveResistancePropertyID, HullKineticResistancePropertyID,
            HullThermalResistancePropertyID, EmDamagePropertyID, ExplosiveDamagePropertyID,
            KineticDamagePropertyID, ThermalDamagePropertyID,
            CharismaModifierPropertyID, IntelligenceModifierPropertyID,
            MemoryModifierPropertyID, PerceptionModifierPropertyID,
            WillpowerModifierPropertyID, MetaLevelPropertyID
        });

        public static ReadOnlyCollection<int> LauncherGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            LauncherGroupPropertyID, LauncherGroup2PropertyID, LauncherGroup3PropertyID
        });

        public static ReadOnlyCollection<int> ChargeGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            ChargeGroup1PropertyID, ChargeGroup2PropertyID, ChargeGroup3PropertyID,
            ChargeGroup4PropertyID, ChargeGroup5PropertyID
        });

        public static ReadOnlyCollection<int> CanFitShipGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            CanFitShipGroup1PropertyID, CanFitShipGroup2PropertyID,
            CanFitShipGroup3PropertyID, CanFitShipGroup4PropertyID
        });

        public static ReadOnlyCollection<int> ModuleShipGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            ModuleShipGroup1PropertyID, ModuleShipGroup2PropertyID,
            ModuleShipGroup3PropertyID
        });

        public static ReadOnlyCollection<int> ReactionGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            ReactionGroup1PropertyID, ReactionGroup2PropertyID
        });

        public static ReadOnlyCollection<int> IndustryModifyingPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            ManufacturingTimeBonusPropertyID, ManufactureCostBonusPropertyID,
            CopySpeedBonusPropertyID, BlueprintManufactureTimeBonusPropertyID,
            MineralNeedResearchBonusPropertyID
        });

        public static ReadOnlyCollection<int> SpecialisationAsteroidGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            SpecialisationAsteroidGroupPropertyID
        });

        public static ReadOnlyCollection<int> PosCargobayAcceptGroupPropertyIDs => new ReadOnlyCollection<int>(new[]
        {
            PosCargobayAcceptGroupPropertyID
        });

        // Group of MarketGroupIDs
        public static ReadOnlyCollection<int> StrategicComponentsMarketGroupIDs
            => new ReadOnlyCollection<int>(new[] { SubsystemsMarketGroupID, StrategicCruisersMarketGroupID });

        public static ReadOnlyCollection<int> SmallToLargeShipsMarketGroupIDs => new ReadOnlyCollection<int>(new[]
        {
            ShuttlessMarketGroupID, StandardFrigatesMarketGroupID, StandardDestroyersMarketGroupID,
            FightersMarketGroupID, StandardCruisersMarketGroupID,
            StandardBattlecruisersMarketGroupID, StandardBattleshipsMarketGroupID,
            StandardIndustrialShipsMarketGroupID,
            MiningBargesMarketGroupID, FreightersMarketGroupID
        });

        public static ReadOnlyCollection<int> AdvancedSmallToLargeShipsMarketGroupIDs => new ReadOnlyCollection<int>(new[]
        {
            InterceptorsMarketGroupID, CovertOpsMarketGroupID, AssaultShipsMarketGroupID,
            LogisticsMarketGroupID, HeavyAssaultShipsMarketGroupID,
            TransportShipsMarketGroupID, CommandShipsMarketGroupID, InterdictorsMarketGroupID,
            ReconShipsMarketGroupID, ExhumersMarketGroupID,
            ElectronicAttackFrigatesMarketGroupID, HeavyInterdictorsMarketGroupID,
            BlackOpsMarketGroupID, MaraudersMarketGroupID, JumpFreightersMarketGroupID
        });

        public static ReadOnlyCollection<int> CapitalShipsMarketGroupIDs => new ReadOnlyCollection<int>(new[]
        {
            StandardBattleshipsMarketGroupID, DreadnoughtsMarketGroupID, FreightersMarketGroupID,
            CarriersMarketGroupID, CapitalIndustrialShipsMarketGroupID, ForceAuxiliariesMarketGroupID
        });

        public static ReadOnlyCollection<int> SupercapitalShipsMarketGroupIDs => new ReadOnlyCollection<int>(new[]
        {
            DreadnoughtsMarketGroupID, FreightersMarketGroupID, CarriersMarketGroupID, ForceAuxiliariesMarketGroupID,
            CapitalIndustrialShipsMarketGroupID, JumpFreightersMarketGroupID, TitansMarketGroupID
        });

        // Group of Implants IDs
        public static ReadOnlyCollection<int> ManufacturingModifyingImplantIDs => new ReadOnlyCollection<int>(new[]
        {
            ZainouBeancounterBX801ID, ZainouBeancounterBX802ID, ZainouBeancounterBX804ID
        });

        public static ReadOnlyCollection<int> ResearchMaterialEfficiencyTimeModifyingImplantIDs => new ReadOnlyCollection<int>(new[]
        {
            ZainouBeancounterMY701ID, ZainouBeancounterMY703ID, ZainouBeancounterMY705ID
        });

        public static ReadOnlyCollection<int> ResearchCopyTimeModifyingImplantIDs => new ReadOnlyCollection<int>(new[]
        {
            ZainouBeancounterSC801ID, ZainouBeancounterSC803ID, ZainouBeancounterSC805ID
        });

        public static ReadOnlyCollection<int> ResearchTimeEfficiencyTimeModifyingImplantIDs => new ReadOnlyCollection<int>(new[]
        {
            ZainouBeancounterRR601ID, ZainouBeancounterRR603ID, ZainouBeancounterRR605ID
        });

        // Group of Faction IDs
        public static ReadOnlyCollection<int> FactionIDs => new ReadOnlyCollection<int>(new[]
        {
            CaldariFactionID, MinmatarFactionID, AmarrFactionID,
            GallenteFactionID, JoveFactionID, ConcordAssemblyFactionID,
            AmmatarMandateFactionID, KhanidKingdomFactionID, TheSyndicateFactionID,
            GuristasPiratesFactionID, AngelCartelFactionID, BloodRaiderCovenantFactionID,
            TheInterBusFactionID, OREFactionID, ThukkerTribeFactionID,
            ServantSistersofEVEFactionID, TheSocietyofConsciousThoughtFactionID,
            MordusLegionCommandFactionID, SanshasNationFactionID, SerpentisFactionID
        });

        // Group of Faction Market Group IDs
        public static ReadOnlyCollection<int> FactionMarketGroupIDs => new ReadOnlyCollection<int>(new[]
        {
            FactionFrigatesMarketGroupID, FactionCruisersMarketGroupID,
            FactionBattleshipsMarketGroupID, FactionCarrierMarketGroupID
        });

        // Group of Extarvtor COntrol Unit Type IDs
        public static ReadOnlyCollection<int> EcuTypeIDs => new ReadOnlyCollection<int>(new[]
        {
            BarrenExtractorControlUnit, GasExtractorControlUnit, IceExtractorControlUnit,
            LavaExtractorControlUnit, OceanicExtractorControlUnit, PlasmaExtractorControlUnit,
            StormExtractorControlUnit, TemperateExtractorControlUnit
        });

        #endregion


        #region Effect IDs

        public const int LowSlotEffectID = 11;
        public const int HiSlotEffectID = 12;
        public const int MedSlotEffectID = 13;

        #endregion


        #region Faction IDs

        public const int CaldariFactionID = 500001;
        public const int MinmatarFactionID = 500002;
        public const int AmarrFactionID = 500003;
        public const int GallenteFactionID = 500004;
        public const int JoveFactionID = 500005;
        public const int ConcordAssemblyFactionID = 500006;
        public const int AmmatarMandateFactionID = 500007;
        public const int KhanidKingdomFactionID = 500008;
        public const int TheSyndicateFactionID = 500009;
        public const int GuristasPiratesFactionID = 500010;
        public const int AngelCartelFactionID = 500011;
        public const int BloodRaiderCovenantFactionID = 500012;
        public const int TheInterBusFactionID = 500013;
        public const int OREFactionID = 500014;
        public const int ThukkerTribeFactionID = 500015;
        public const int ServantSistersofEVEFactionID = 500016;
        public const int TheSocietyofConsciousThoughtFactionID = 500017;
        public const int MordusLegionCommandFactionID = 500018;
        public const int SanshasNationFactionID = 500019;
        public const int SerpentisFactionID = 500020;

        #endregion


        #region Attribute Category IDs

        public const int FittingAtributeCategoryID = 1;
        public const int StructureAtributeCategoryID = 4;
        public const int MiscellaneousAttributeCategoryID = 7;
        public const int NullAtributeCategoryID = 9;
        public const int SpeedAtributeCategoryID = 17;

        #endregion


        #region Attribute types IDs (Properties)

        public const int MassPropertyID = 4;
        public const int CapacitorNeedPropertyID = 6;
        public const int StructureHitpointsPropertyID = 9;
        public const int PGOutputPropertyID = 11;
        public const int LowSlotsPropertyID = 12;
        public const int MedSlotsPropertyID = 13;
        public const int HiSlotsPropertyID = 14;
        public const int PGNeedPropertyID = 30;
        public const int MaxVelocityPropertyID = 37;
        public const int CargoCapacityPropertyID = 38;
        public const int CPUOutputPropertyID = 48;
        public const int CPUNeedPropertyID = 50;
        public const int CapacitorRechargeRatePropertyID = 55;
        public const int ShieldBonusPropertyID = 68;
        public const int AgilityPropertyID = 70;
        public const int DurationPropertyID = 73;
        public const int MaxTargetRangePropertyID = 76;
        public const int ScanSpeedPropertyID = 79;
        public const int ShieldTransferRangePropertyID = 87;
        public const int LauncherSlotsLeftPropertyID = 101;
        public const int TurretSlotsLeftPropertyID = 102;
        public const int EmDamagePropertyID = 114;
        public const int ExplosiveDamagePropertyID = 116;
        public const int KineticDamagePropertyID = 117;
        public const int ThermalDamagePropertyID = 118;
        public const int UniformityPropertyID = 136;
        public const int LauncherGroupPropertyID = 137;
        public const int CapacitorRechargeRateMultiplierPropertyID = 144;
        public const int VolumePropertyID = 161;
        public const int CharismaPropertyID = 164;
        public const int IntelligencePropertyID = 165;
        public const int MemoryPropertyID = 166;
        public const int PerceptionPropertyID = 167;
        public const int WillpowerPropertyID = 168;
        public const int CharismaModifierPropertyID = 175;
        public const int IntelligenceModifierPropertyID = 176;
        public const int MemoryModifierPropertyID = 177;
        public const int PerceptionModifierPropertyID = 178;
        public const int WillpowerModifierPropertyID = 179;
        public const int PrimaryAttributePropertyID = 180;
        public const int SecondaryAttributePropertyID = 181;
        public const int RequiredSkill1PropertyID = 182;
        public const int RequiredSkill2PropertyID = 183;
        public const int RequiredSkill3PropertyID = 184;
        public const int CPUOutputBonusPropertyID = 202;
        public const int ScanRadarStrengthPropertyID = 208;
        public const int ScanLadarStrengthPropertyID = 209;
        public const int ScanMagnetometricStrengthPropertyID = 210;
        public const int ScanGravimetricStrengthPropertyID = 211;
        public const int ShieldHitpointsPropertyID = 263;
        public const int ArmorHitpointsPropertyID = 265;
        public const int ArmorEMResistancePropertyID = 267;
        public const int ArmorExplosiveResistancePropertyID = 268;
        public const int ArmorKineticResistancePropertyID = 269;
        public const int ArmorThermalResistancePropertyID = 270;
        public const int ShieldEMResistancePropertyID = 271;
        public const int ShieldExplosiveResistancePropertyID = 272;
        public const int ShieldKineticResistancePropertyID = 273;
        public const int ShieldThermalResistancePropertyID = 274;
        public const int SkillTimeConstantPropertyID = 275;
        public const int RequiredSkill1LevelPropertyID = 277;
        public const int RequiredSkill2LevelPropertyID = 278;
        public const int RequiredSkill3LevelPropertyID = 279;
        public const int DroneCapacityPropertyID = 283;
        public const int ImplantSlotPropertyID = 331;
        public const int TechLevelPropertyID = 422;
        public const int CPUOutputBonus2PropertyID = 424;
        public const int ManufacturingTimeBonusPropertyID = 440;
        public const int ManufactureCostBonusPropertyID = 451;
        public const int CopySpeedBonusPropertyID = 452;
        public const int BlueprintManufactureTimeBonusPropertyID = 453;
        public const int MineralNeedResearchBonusPropertyID = 468;
        public const int ShieldRechargeRatePropertyID = 479;
        public const int CapacitorCapacityPropertyID = 482;
        public const int ShieldUniformityPropertyID = 484;
        public const int ArmorUniformityPropertyID = 524;
        public const int StructureUniformityPropertyID = 525;
        public const int SignatureRadiusPropertyID = 552;
        public const int AnchoringDelayPropertyID = 556;
        public const int CloakingTargetingDelayPropertyID = 560;
        public const int ScanResolutionPropertyID = 564;
        public const int WarpSpeedMultiplierPropertyID = 600;
        public const int LauncherGroup2PropertyID = 602;
        public const int LauncherGroup3PropertyID = 603;
        public const int ChargeGroup1PropertyID = 604;
        public const int ChargeGroup2PropertyID = 605;
        public const int ChargeGroup3PropertyID = 606;
        public const int ChargeGroup4PropertyID = 609;
        public const int ChargeGroup5PropertyID = 610;
        public const int MetaLevelPropertyID = 633;
        public const int ModuleShipGroup2PropertyID = 666;
        public const int ModuleShipGroup3PropertyID = 667;
        public const int ModuleShipGroup1PropertyID = 668;
        public const int ModuleReactivationDelayPropertyID = 669;
        public const int UnanchoringDelayPropertyID = 676;
        public const int OnliningDelayPropertyID = 677;
        public const int ConsumptionQuantityPropertyID = 714;
        public const int MoonMiningAmountPropertyID = 726;
        public const int IceHarvestCycleBonusPropertyID = 780;
        public const int SpecialisationAsteroidGroupPropertyID = 781;
        public const int ReprocessingSkillPropertyID = 790;
        public const int ReactionGroup1PropertyID = 842;
        public const int ReactionGroup2PropertyID = 843;
        public const int ShipMaintenanceBayCapacityPropertyID = 908;
        public const int HullEMResistancePropertyID = 974;
        public const int HullExplosiveResistancePropertyID = 975;
        public const int HullKineticResistancePropertyID = 976;
        public const int HullThermalResistancePropertyID = 977;
        public const int CanNotBeTrainedOnTrialPropertyID = 1047;
        public const int CPUPenaltyPercentPropertyID = 1082;
        public const int UpgradeCapacityPropertyID = 1132;
        public const int RigSlotsPropertyID = 1137;
        public const int UpgradeCostPropertyID = 1153;
        public const int DroneBandwidthPropertyID = 1271;
        public const int DroneBandwidthUsedPropertyID = 1272;
        public const int ShipWarpSpeedPropertyID = 1281;
        public const int RequiredSkill4PropertyID = 1285;
        public const int RequiredSkill4LevelPropertyID = 1286;
        public const int RequiredSkill5LevelPropertyID = 1287;
        public const int RequiredSkill6LevelPropertyID = 1288;
        public const int RequiredSkill5PropertyID = 1289;
        public const int RequiredSkill6PropertyID = 1290;
        public const int CanFitShipGroup1PropertyID = 1298;
        public const int CanFitShipGroup2PropertyID = 1299;
        public const int CanFitShipGroup3PropertyID = 1300;
        public const int CanFitShipGroup4PropertyID = 1301;
        public const int PosCargobayAcceptGroupPropertyID = 1352;
        public const int MaxSubSystemsPropertyID = 1367;
        public const int TurretHardPointModifierPropertyID = 1368;
        public const int LauncherHardPointModifierPropertyID = 1369;
        public const int HiSlotModifierPropertyID = 1374;
        public const int MedSlotModifierPropertyID = 1375;
        public const int LowSlotModifierPropertyID = 1376;
        public const int FitsToShipTypePropertyID = 1380;
        public const int AITargetSwitchTimerPropertyID = 1416;
        public const int RigSizePropertyID = 1547;
        public const int MetaGroupPropertyID = 1692;

        #endregion


        #region EVE Unit IDs (Properties UnitID's)

        public const int MassUnitID = 2;
        public const int MillsecondsUnitID = 101;
        public const int AbsolutePercentUnitID = 127;
        public const int InverseAbsolutePercentUnitID = 108;
        public const int ModifierPercentUnitID = 109;
        public const int InversedModifierPercentUnitID = 111;
        public const int GroupIDUnitID = 115;
        public const int TypeUnitID = 116;
        public const int SizeclassUnitID = 117;

        #endregion


        #region Category IDs

        public const int ShipCategoryID = 6;
        public const int BlueprintCategoryID = 9;
        public const int SkillCategoryID = 16;
        public const int ImplantCategoryID = 20;
        public const int ReactionCategoryID = 24;
        public const int AsteroidCategoryID = 25;
        public const int AncientRelicsCategoryID = 34;

        #endregion


        #region Group IDs

        public const int FrigateGroupID = 25;
        public const int CruiserGroupID = 26;
        public const int BattleshipGroupID = 27;
        public const int IndustrialGroupID = 28;
        public const int TitanGroupID = 30;
        public const int RookieShipGroupID = 237;
        public const int TradeSkillsGroupID = 274;
        public const int SocialSkillsGroupID = 278;
        public const int AssaultFrigateGroupID = 324;
        public const int HeavyAssaultCruiserGroupID = 358;
        public const int ControlTowerGroupID = 365;
        public const int DeepSpaceTransportGroupID = 380;
        public const int EliteBattleshipGroupID = 381;
        public const int CombatBattlecruiserGroupID = 419;
        public const int DestroyerGroupID = 420;
        public const int MiningBargeGroupID = 463;
        public const int DreadnoughtGroupID = 485;
        public const int FakeSkillsGroupID = 505;
        public const int FreighterGroupID = 513;
        public const int CommandShipGroupID = 540;
        public const int InterdictorGroupID = 541;
        public const int ExhumerGroupID = 543;
        public const int CarrierGroupID = 547;
        public const int SupercarrierGroupID = 659;
        public const int CyberLearningImplantsGroupID = 745;
        public const int CovertOpsGroupID = 830;
        public const int InterceptorGroupID = 831;
        public const int LogisticsGroupID = 832;
        public const int ForceReconShipGroupID = 833;
        public const int StealthBomberGroupID = 834;
        public const int CapitalIndustrialShipGroupID = 883;
        public const int ElectronicAttackShipGroupID = 893;
        public const int HeavyInterdictorCruiserGroupID = 894;
        public const int BlackOpsGroupID = 898;
        public const int MarauderGroupID = 900;
        public const int JumpFreighterGroupID = 902;
        public const int CombatReconShipGroupID = 906;
        public const int IndustrialCommandShipGroupID = 941;
        public const int StrategicCruiserGroupID = 963;
        public const int CorporationManagementSkillsGroupID = 266;
        public const int AttackBattlecruiserGroupID = 1201;
        public const int BlockadeRunnerGroupID = 1202;
        public const int ExpeditionFrigateGroupID = 1283;
        public const int TacticalDestroyerGroupID = 1305;

        #endregion


        #region Market group IDs

        public const int BlueprintsMarketGroupID = 2;
        public const int ShipsMarketGroupID = 4;
        public const int StandardFrigatesMarketGroupID = 5;
        public const int StandardCruisersMarketGroupID = 6;
        public const int StandardBattleshipsMarketGroupID = 7;
        public const int StandardIndustrialShipsMarketGroupID = 8;
        public const int ShipEquipmentsMarketGroupID = 9;
        public const int AmmosAndChargesMarketGroupID = 11;
        public const int ImplantsAndBoostersMarketGroupID = 24;
        public const int ImplantsMarketGroupID = 27;
        public const int SkillsMarketGroupID = 150;
        public const int DronesMarketGroupID = 157;
        public const int ShuttlessMarketGroupID = 391;
        public const int InterceptorsMarketGroupID = 399;
        public const int CovertOpsMarketGroupID = 420;
        public const int AssaultShipsMarketGroupID = 432;
        public const int LogisticsMarketGroupID = 437;
        public const int HeavyAssaultShipsMarketGroupID = 448;
        public const int StandardDestroyersMarketGroupID = 464;
        public const int StandardBattlecruisersMarketGroupID = 469;
        public const int ComponentsMarketGroupID = 475;
        public const int StarbaseStructuresMarketGroupID = 477;
        public const int MiningBargesMarketGroupID = 494;
        public const int TransportShipsMarketGroupID = 629;
        public const int DreadnoughtsMarketGroupID = 761;
        public const int FreightersMarketGroupID = 766;
        public const int StandardCapitalShipComponentsMarketGroupID = 781;
        public const int TitansMarketGroupID = 812;
        public const int CarriersMarketGroupID = 817;
        public const int CommandShipsMarketGroupID = 822;
        public const int InterdictorsMarketGroupID = 823;
        public const int ReconShipsMarketGroupID = 824;
        public const int ExhumersMarketGroupID = 874;
        public const int ShipModificationsMarketGroupID = 955;
        public const int BoostersMarketGroupID = 977;
        public const int CapitalIndustrialShipsMarketGroupID = 1047;
        public const int ElectronicAttackFrigatesMarketGroupID = 1065;
        public const int HeavyInterdictorsMarketGroupID = 1070;
        public const int BlackOpsMarketGroupID = 1075;
        public const int MaraudersMarketGroupID = 1080;
        public const int JumpFreightersMarketGroupID = 1089;
        public const int SubsystemsMarketGroupID = 1112;
        public const int StrategicCruisersMarketGroupID = 1138;
        public const int FactionFrigatesMarketGroupID = 1362;
        public const int FactionCruisersMarketGroupID = 1369;
        public const int FactionBattleshipsMarketGroupID = 1378;
        public const int FactionCarrierMarketGroupID = 1392;
        public const int FuelBlocksMarketGroupID = 1870;
        public const int DatacoresMarketGroupID = 1880;
        public const int AdvancedCapitalComponentsMarketGroupID = 1883;
        public const int AncientRelicsMarketGroupID = 1909;
        public const int FightersMarketGroupID = 2236;
        public const int ForceAuxiliariesMarketGroupID = 2271;

        #endregion


        #region Custom market group IDs

        public const int RootNonMarketGroupID = 11000;

        public const int UniqueDesignsRootNonMarketGroupID = 10000;

        public const int BlueprintRootNonMarketGroupID = 21000;
        public const int BlueprintTechINonMarketGroupID = 21001;
        public const int BlueprintTechIINonMarketGroupID = 21002;
        public const int BlueprintStorylineNonMarketGroupID = 21003;
        public const int BlueprintFactionNonMarketGroupID = 21004;
        public const int BlueprintOfficerNonMarketGroupID = 21005;
        public const int BlueprintTechIIINonMarketGroupID = 21014;
        public const int ResearchEquipmentNonMarketGroupID = 21100;

        #endregion


        #region MetaGroup IDs

        public const int TechIMetaGroupID = 1;
        public const int TechIIMetaGroupID = 2;
        public const int StorylineMetaGroupID = 3;
        public const int FactionMetaGroupID = 4;
        public const int OfficerMetaGroupID = 5;
        public const int DeadspaceMetaGroupID = 6;
        public const int TechIIIMetaGroupID = 14;

        #endregion


        #region TechLevels

        public const int TechLevelII = 2;
        public const int TechLevelIII = 3;

        #endregion


        #region Icon IDs

        public const int UnknownIconID = 0;
        public const int UnknownShipIconID = 1443;
        public const int UnknownBlueprintBackdropIconID = 2703;

        #endregion


        #region Types IDs

        public const int CorporationID = 2;
        public const int CapsuleID = 670;
        public const int CharacterAmarrID = 1373;
        public const int CharacterVherokiorID = 1386;
        public const int AdrestiaBlueprintID = 2837;
        public const int BarrenExtractorControlUnit = 2848;
        public const int GasExtractorControlUnit = 3060;
        public const int IceExtractorControlUnit = 3061;
        public const int LavaExtractorControlUnit = 3062;
        public const int OceanicExtractorControlUnit = 3063;
        public const int PlasmaExtractorControlUnit = 3064;
        public const int StormExtractorControlUnit = 3067;
        public const int TemperateExtractorControlUnit = 3068;
        public const int GunnerySkillID = 3300;
        public const int SmallHybridTurretSkillID = 3301;
        public const int SmallProjectileTurretSkillID = 3302;
        public const int SmallEnergyTurretSkillID = 3303;
        public const int RapidFiringSkillID = 3310;
        public const int SharpshooterSkillID = 3311;
        public const int MotionPredictionSkillID = 3312;
        public const int SurgicalStrikeSkillID = 3315;
        public const int ControlledBurstsSkillID = 3316;
        public const int TrajectoryAnalysisSkillID = 3317;
        public const int WeaponUpgradesSkillID = 3318;
        public const int MissileLauncherOperationSkillID = 3319;
        public const int GallenteIndustrialSkillID = 3340;
        public const int MinmatarIndustrialSkillID = 3341;
        public const int CaldariIndustrialSkillID = 3342;
        public const int AmarrIndustrialSkillID = 3343;
        public const int SpaceshipCommandSkillID = 3327;
        public const int GallenteFrigateSkillID = 3328;
        public const int MinmatarFrigateSkillID = 3329;
        public const int CaldariFrigateSkillID = 3330;
        public const int AmarrFrigateSkillID = 3331;
        public const int DiplomacySkillID = 3357;
        public const int ConnectionsSkillID = 3359;
        public const int IndustrySkillID = 3380;
        public const int MiningSkillID = 3386;
        public const int MassProductionSkillID = 3387;
        public const int AdvancedIndustrySkillID = 3388;
        public const int MechanicSkillID = 3392;
        public const int RepairSystemsSkillID = 3393;
        public const int HullUpgradesSkillID = 3394;
        public const int ScienceSkillID = 3402;
        public const int ResearchSkillID = 3403;
        public const int LaboratoryOperationSkillID = 3406;
        public const int MetallurgySkillID = 3409;
        public const int CyberneticsSkillID = 3411;
        public const int AstrometricsSkillID = 3412;
        public const int PowerGridManagementSkillID = 3413;
        public const int ShieldOperationSkillID = 3416;
        public const int CapacitorSystemsOperationSkillID = 3417;
        public const int CapacitorManagementSkillID = 3418;
        public const int ShieldManagementSkillID = 3419;
        public const int TacticalShieldManipulationSkillID = 3420;
        public const int EnergyGridUpgradesSkillID = 3424;
        public const int ShieldUpgradesSkillID = 3425;
        public const int CPUManagementSkillID = 3426;
        public const int ElectronicWarfareSkillID = 3427;
        public const int LongRangeTargetingSkillID = 3428;
        public const int TargetManagementSkillID = 3429;
        public const int SignatureAnalysisSkillID = 3431;
        public const int ElectronicsUpgradesSkillID = 3432;
        public const int PropulsionJammingSkillID = 3435;
        public const int DronesSkillID = 3436;
        public const int DroneAvionicsSkillID = 3437;
        public const int TradeSkillID = 3443;
        public const int RetailSkillID = 3444;
        public const int BrokerRelationsSkillID = 3446;
        public const int VisibilitySkillID = 3447;
        public const int NavigationSkillID = 3449;
        public const int AfterburnerSkillID = 3450;
        public const int SurveySkillID = 3551;
        public const int AccelerationControlSkillID = 3452;
        public const int EvasiveManeuveringSkillID = 3453;
        public const int HighSpeedManeuveringSkillID = 3454;
        public const int WarpDriveOperationSkillID = 3455;
        public const int EchelonBlueprintID = 3533;
        public const int ScrapMetalProcessingSkillID = 12196;
        public const int ArchaeologySkillID = 13278;
        public const int AllianceID = 16159;
        public const int ProcurementSkillID = 16594;
        public const int DaytradingSkillID = 16595;
        public const int WholesaleSkillID = 16596;
        public const int MarketingSkillID = 16598;
        public const int AccountingSkillID = 16622;
        public const int CaldariNavyHookbillBlueprintID = 17620;
        public const int ImperialNavySlicerBlueprintID = 17704;
        public const int PhantasmBlueprintID = 17719;
        public const int CynabalBlueprintID = 17721;
        public const int NightmareBlueprintID = 17737;
        public const int MacharielBlueprintID = 17739;
        public const int RepublicFleetFiretailBlueprintID = 17813;
        public const int FederationNavyCometBlueprintID = 17842;
        public const int AshimmuBlueprintID = 17923;
        public const int SuccubusBlueprintID = 17925;
        public const int CruorBlueprintID = 17927;
        public const int DaredevilBlueprintID = 17929;
        public const int DramielBlueprintID = 17933;
        public const int TycoonSkillID = 18580;
        public const int GorusShuttleBlueprintID = 21098;
        public const int GuristasShuttleBlueprintID = 21629;
        public const int HackingSkillID = 21718;
        public const int GallenteMiningLaserBlueprintID = 21842;
        public const int WildMinerIBlueprintID = 22924;
        public const int InfomorphPsychologySkillID = 24242;
        public const int SupplyChainManagementSkillID = 24268;
        public const int ScientificNetworkingSkillID = 24270;
        public const int AdvancedLaboratoryOperationSkillID = 24624;
        public const int AdvancedMassProductionSkillID = 24625;
        public const int ReactionsSkillID = 45746;
        public const int MassReactionsSkillID = 45748;
        public const int AdvancedMassReactionsSkillID = 45749;
        public const int RemoteReactionsSkillID = 45750;
        public const int AstrometricRangefindingSkillID = 25739;
        public const int AstrometricAcquisitionSkillID = 25811;
        public const int SalvagingSkillID = 25863;
        public const int ZainouBeancounterBX802ID = 27167;
        public const int ZainouBeancounterBX801ID = 27170;
        public const int ZainouBeancounterBX804ID = 27171;
        public const int ZainouBeancounterMY703ID = 27176;
        public const int ZainouBeancounterRR603ID = 27177;
        public const int ZainouBeancounterSC803ID = 27178;
        public const int ZainouBeancounterRR605ID = 27179;
        public const int ZainouBeancounterRR601ID = 27180;
        public const int ZainouBeancounterMY705ID = 27181;
        public const int ZainouBeancounterMY701ID = 27182;
        public const int ZainouBeancounterSC805ID = 27184;
        public const int ZainouBeancounterSC801ID = 27185;
        public const int ThermodynamicsSkillID = 28164;
        public const int TenguBlueprintID = 29985;
        public const int LegionBlueprintID = 29987;
        public const int ProteusBlueprintID = 29989;
        public const int LokiBlueprintID = 29991;
        public const int LegionElectronicsEnergyParasiticComplexBlueprintID = 30037;
        public const int TenguElectronicsObfuscationManifoldBlueprintID = 30047;
        public const int ProteusElectronicsFrictionExtensionProcessorBlueprintID = 30057;
        public const int LokiElectronicsImmobilityDriversBlueprintID = 30067;
        public const int LegionPropulsionChassisOptimizationBlueprintID = 30077;
        public const int TenguPropulsionIntercalatedNanofibersBlueprintID = 30087;
        public const int ProteusPropulsionWakeLimiterBlueprintID = 30097;
        public const int LokiPropulsionChassisOptimizationBlueprintID = 30107;
        public const int TenguEngineeringPowerCoreMultiplierBlueprintID = 30140;
        public const int ProteusEngineeringPowerCoreMultiplierBlueprintID = 30150;
        public const int LokiEngineeringPowerCoreMultiplierBlueprintID = 30160;
        public const int LegionEngineeringPowerCoreMultiplierBlueprintID = 30170;
        public const int LegionDefensiveAdaptiveAugmenterBlueprintID = 30227;
        public const int TenguDefensiveAdaptiveShieldingBlueprintID = 30232;
        public const int ProteusDefensiveAdaptiveAugmenterBlueprintID = 30237;
        public const int LokiDefensiveAdaptiveShieldingBlueprintID = 30242;
        public const int LegionOffensiveDroneSynthesisProjectorBlueprintID = 30392;
        public const int TenguOffensiveAcceleratedEjectionBayBlueprintID = 30397;
        public const int ProteusOffensiveDissonicEncodingPlatformBlueprintID = 30402;
        public const int LokiOffensiveTurretConcurrenceRegistryBlueprintID = 30407;
        public const int InterbusShuttleBlueprintID = 30843;
        public const int FrekiBlueprintID = 32208;
        public const int MimirBlueprintID = 32210;
        public const int MiningFrigateSkillID = 32918;

        public const int AlphaDataAnalyzerIBlueprintID = 22330;
        public const int DaemonDataAnalyzerIBlueprintID = 22326;
        public const int CodexDataAnalyzerIBlueprintID = 22328;
        public const int LibramDataAnalyzerIBlueprintID = 22332;
        public const int CropGasCloudHarvesterBlueprintID = 25541;
        public const int PlowGascloudHarvesterBlueprintID = 25543;
        public const int Dual1000mmScoutIAcceleratorCannonBlueprintID = 3557;
        public const int HabitatMinerIBlueprintID = 22922;
        public const int LimosCitadelCruiseLauncherIBlueprintID = 3564;
        public const int MagpieMobileTractorUnitBlueprintID = 33703;
        public const int PackratMobileTractorUnitBlueprintID = 33701;
        public const int ShockLimosCitadelTorpedoBayIBlueprintID = 3570;
        public const int WetuMobileDepotBlueprintID = 33521;
        public const int YurtMobileDepotBlueprintID = 33523;

        public const int AsteroBlueprintID = 33469;
        public const int BarghestBlueprintID = 33821;
        public const int CambionBlueprintID = 32789;
        public const int ChremoasBlueprintID = 33398;
        public const int EtanaBlueprintID = 32791;
        public const int GarmurBlueprintID = 33817;
        public const int MaliceBlueprintID = 3517;
        public const int MorachaBlueprintID = 33396;
        public const int NestorBlueprintID = 33473;
        public const int OrthrusBlueprintID = 33819;
        public const int PolicePursuitCometBlueprintID = 33678;
        public const int ScorpionIshukoneWatchBlueprintID = 4006;
        public const int ShadowBlueprintID = 2949;
        public const int StratiosBlueprintID = 33471;
        public const int StratiosEmergencyResponderBlueprintID = 33554;
        public const int UtuBlueprintID = 2835;
        public const int VangelBlueprintID = 3519;
        public const int WhiptailBlueprintID = 33674;

        public const int BladeBlueprintID = 11374;
        public const int BlazeLBlueprintID = 12813;
        public const int BlazeMBlueprintID = 12811;
        public const int BlazeSBlueprintID = 12562;
        public const int BoltLBlueprintID = 12800;
        public const int BoltMBlueprintID = 12798;
        public const int BoltSBlueprintID = 12617;
        public const int CapitalRemoteCapacitorTransmitterIIBlueprintID = 12224;
        public const int CapitalRemoteShieldBoosterIIBlueprintID = 3619;
        public const int ChameleonBlueprintID = 33676;
        public const int DaggerBlueprintID = 12037;
        public const int DesolationLBlueprintID = 12796;
        public const int DesolationMBlueprintID = 12794;
        public const int DesolationSBlueprintID = 12611;
        public const int DroneDamageRigIIBlueprintID = 26928;
        public const int ErinyeBlueprintID = 11376;
        public const int GathererBlueprintID = 11384;
        public const int HighGradeAscendancyAlphaBlueprintID = 33536;
        public const int HighGradeAscendancyBetaBlueprintID = 33543;
        public const int HighGradeAscendancyGammaBlueprintID = 33545;
        public const int HighGradeAscendancyDeltaBlueprintID = 33547;
        public const int HighGradeAscendancyEpsilonBlueprintID = 33546;
        public const int HighGradeAscendancyOmegaBlueprintID = 33548;
        public const int KisharBlueprintID = 11390;
        public const int LuxLBlueprintID = 12833;
        public const int LuxMBlueprintID = 12831;
        public const int LuxSBlueprintID = 12553;
        public const int MackinawOREDevelopmentEditionBlueprintID = 33684;
        public const int MediumEWDroneRangeAugmentorIIBlueprintID = 32080;
        public const int MidGradeAscenancyAlphaBlueprintID = 33556;
        public const int MidGradeAscenancyBetaBlueprintID = 33558;
        public const int MidGradeAscenancyGammaBlueprintID = 33564;
        public const int MidGradeAscenancyDeltaBlueprintID = 33560;
        public const int MidGradeAscenancyEpsilonBlueprintID = 33562;
        public const int MidGradeAscenancyOmegaBlueprintID = 33566;
        public const int MinerIIChinaBlueprintID = 26607;
        public const int MiningLaserOptimizationIIBlueprintID = 28891;
        public const int MiningLaserRangeIIBlueprintID = 28895;
        public const int ReconProbeLauncherIIBlueprintID = 25772;
        public const int ScanProbeLauncherIIBlueprintID = 17902;
        public const int ShieldTransporterRigIIBlueprintID = 26967;
        public const int ShockLBlueprintID = 12764;
        public const int ShockMBlueprintID = 12770;
        public const int ShockSBlueprintID = 12630;
        public const int SmallEWDroneRangeAugmentorIIBlueprintID = 32078;
        public const int StormLBlueprintID = 12784;
        public const int StormMBlueprintID = 12782;
        public const int StormSBlueprintID = 12628;
        public const int TalismanAlphaBlueprintID = 11074;
        public const int AdvancedInfomorphPsychologySkillID = 33407;
        public const int InfomorphSynchronizingSkillID = 33399;


        #endregion
    }
}
