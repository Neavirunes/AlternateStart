using UnityEngine;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Questing;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallWorkshop.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AlternateStart
{
	public class AlternateStartMod : MonoBehaviour, IHasModSaveData
	{
		public static int DrumPlayed;
		public static int LutePlayed;
		public static int MugUsed;
		public static int RecorderPlayed;
		public static int ShelfUsed;
		public static PlayerEntity Player;

		static AlternateStartMod ModInstance;
		static bool TeleportMe;
		static DaggerfallMessageBox MessageBox;
		static DaggerfallMessageBox.MessageBoxButtons Yes = (DaggerfallMessageBox.MessageBoxButtons)3;
		static DaggerfallMessageBox.MessageBoxButtons No = (DaggerfallMessageBox.MessageBoxButtons)4;
		static DaggerfallMessageBox.MessageBoxButtons Cancel = (DaggerfallMessageBox.MessageBoxButtons)2;
		static DaggerfallUnityItem AItem;
		static DaggerfallUnityItem BItem;
		static DaggerfallUnityItem Book;
		static DaggerfallUnityItem CItem;
		static DaggerfallUnityItem DItem;
		static DaggerfallUnityItem EItem;
		static DFLocation Location;
		static DFPosition Pixel;
		static DFPosition Position;
		static DFRegion ASRegion;
		static GameObject Bookshelf;
		static Genders AGender;
		static GuildManager GManager;
		static float FAgility;
		static float FEndurance;
		static float FEtiquette;
		static float FIntelligence;
		static float FLevel;
		static float FLuck;
		static float FPersonality;
		static float FStreetwise;
		static float FStrength;
		static float FWillpower;
		static float MaxGoldF;
		static float MinuteDayF;
		static float RandoF;
		static IGuild Guild;
		static int BookCount;
		static int BonusGold;
		static int Combat;
		static int ComMag;
		static int DaysPassed;
		static int ExperienceYears;
		static int GameStarted;
		static int HasInited;
		static int IAgility;
		static int IEndurance;
		static int IEtiquette;
		static int IIntelligence;
		static int ILevel;
		static int ILuck;
		static int Index;
		static int IPersonality;
		static int ISpeed;
		static int IStreetwise;
		static int IStrength;
		static int IWillpower;
		static int Magery;
		static int MaxGoldI;
		static int MinuteDayI;
		static int NewTime;
		static int NPCCheck;
		static int RandoA;
		static int RandoI;
		static int RandomCheck;
		static int VampWolf;
		static ItemCollection Backpack;
		static ItemData_v1 BookData;
		static List<int> Locations;
		static MobileTypes Mobile;
		static Mod ASMod;
		static ModSettings Settings;
		static PlayerEntity.Crimes Crime;
		static Races ARace;
		static StartGameBehaviour GameBehaviour;
		static string KnightOrderName;
		static string Message1;
		static string Message2;
		static System.Random Fortuna;

		// - Knightly Orders ---------------------------------------------------
		static string Knights1 = "Host of the Horn";
		static string Knights2 = "Knights of the Dragon";
		static string Knights3 = "Knights of the Flame";
		static string Knights4 = "Knights of the Hawk";
		static string Knights5 = "Knights of the Owl";
		static string Knights6 = "Knights of the Rose";
		static string Knights7 = "Knights of the Wheel";
		static string Knights8 = "Order of the Candle";
		static string Knights9 = "Order of the Raven";
		static string Knights10 = "Order of the Scarab";
		// ---------------------------------------------------------------------

		// - Religious Orders --------------------------------------------------
		static string Religious1 = "Akatosh Chantry";
		static string Religious2 = "Benevolence of Mara";
		static string Religious3 = "House of Dibella";
		static string Religious4 = "Order of Arkay";
		static string Religious5 = "Resolution of Z'en";
		static string Religious6 = "School of Julianos";
		static string Religious7 = "Temple of Kynareth";
		static string Religious8 = "Temple of Stendarr";
		// ---------------------------------------------------------------------

		// - Save Variables ----------------------------------------------------
		public static int Occupation;
		static float DrumSkill;
		static float LuteSkill;
		static float RecorderSkill;
		static float BeggingSkill;
		public static int LawbookUsed;
		static int SpawnCount;
		static int HasItems;
		public static int ShelfExists;
		static int ShelfValue;
		static string ShelfName;
		static List<DaggerfallUnityItem> Shelf;
		static List<ItemData_v1> Data;
		static Vector3 ShelfPosition;
		static Quaternion ShelfRotation;
		public static int TemplateIndex;
		public static int TextureIndex;
		static int OldTime;
		// ---------------------------------------------------------------------

		[Invoke(StateManager.StateTypes.Start, 0)]
		public static void Init(InitParams initParams)
		{
			ASMod = initParams.Mod;
			var go = new GameObject(ASMod.Title);
			ModInstance = go.AddComponent<AlternateStartMod>();

			Occupation = Settings.GetValue<int>("General", "Occupation");
			KnightOrderName = Settings.GetValue<string>("General", "KnightOrderName");
			ExperienceYears = Settings.GetValue<int>("General", "ExperienceYears");
			VampWolf = Settings.GetValue<int>("General", "VampWolf");
			TeleportMe = Settings.GetValue<bool>("General", "TeleportMe");

			DrumSkill = (float)ExperienceYears;
			LuteSkill = (float)ExperienceYears;
			RecorderSkill = (float)ExperienceYears;
			BeggingSkill = (float)ExperienceYears;
			LawbookUsed = 0;
			SpawnCount = 0;
			HasItems = 0;
			ShelfExists = 0;
			ShelfValue = 0;
			ShelfName = "";
			Shelf = new List<DaggerfallUnityItem>();
			Data = new List<ItemData_v1>();
			ShelfPosition = new Vector3();
			ShelfRotation = new Quaternion();
			TemplateIndex = 0;
			TextureIndex = 0;
			OldTime = 0;

			ASMod.SaveDataInterface = ModInstance;
		}

		void Awake()
		{
			InitMod();
			ASMod.IsReady = true;
		}

		void Update()
		{
			if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Game)
			{
				if (GameStarted == 1)
				{
					if (Player.GoldPieces > 0)
					{
						Backpack = Player.Items;
						Fortuna = new System.Random();

						switch(Occupation)
						{
							case 0:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42446));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for alchemists.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 1:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42451));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownVillage)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for armorers.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 2:
								Guild = GManager.JoinGuild((FactionFile.GuildGroups)5, 0);

								IStrength = Player.Stats.LiveStrength;
								IAgility = Player.Stats.LiveAgility;
								ISpeed = Player.Stats.LiveSpeed;
								Combat = (IStrength + IAgility + ISpeed) / 3;

								IIntelligence = Player.Stats.LiveIntelligence;
								IWillpower = Player.Stats.LiveWillpower;
								Magery = (IIntelligence + IWillpower) / 2;

								IEndurance = Player.Stats.LiveEndurance;
								IPersonality = Player.Stats.LivePersonality;
								ILuck = Player.Stats.LiveLuck;
								ComMag = (IEndurance + IPersonality + ILuck) / 3;

								if (Combat > Magery)
								{
									if (Combat > ComMag)
									{
										Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42440));
									}
									else
									{
										Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42442));
									}
								}
								else
								{
									if (Magery > ComMag)
									{
										Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42441));
									}
									else
									{
										Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42442));
									}
								}

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.Tavern)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for bards.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 3:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42443));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for beggars.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 4:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.Books, 42444));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownVillage)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for guards.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 5:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42452));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for jewellers.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 6:
								if (Knights1.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 411);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights2.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 368);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights3.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 410);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights4.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 417);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights5.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 413);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights6.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 409);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights7.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 415);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights8.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 408);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights9.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 414);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								if (Knights10.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)9, 416);
									GManager.AddMembership((FactionFile.GuildGroups)9, Guild);
								}

								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;

								RandoF = Fortuna.Next(0, 10);
								RandoF *= FLevel;
								RandoI = (int)RandoF;

								Guild.Rank = RandoI;
								Guild.UpdateRank(Player);

								Message1 = "You are a member of a knightly " +
								"order. " + string.Format("{0}", KnightOrderName) +
								". Your rank is " + string.Format("{0}", RandoI) + "/10.";
								MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
								MessageBox.SetText(Message1, null);
								MessageBox.ClickAnywhereToClose = true;
								MessageBox.AllowCancel = false;
								MessageBox.Show();

								break;
							case 7:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42445));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownHamlet)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for librarians.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 8:
								Guild = GManager.JoinGuild((FactionFile.GuildGroups)11, 41);
								GManager.AddMembership((FactionFile.GuildGroups)11, Guild);

								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;

								RandoF = Fortuna.Next(0, 10);
								RandoF *= FLevel;
								RandoI = (int)RandoF;

								Guild.Rank = RandoI;
								Guild.UpdateRank(Player);

								Message1 = "You are a member of the Fighters " +
								"Guild. Your rank is " + string.Format("{0}", RandoI) + "/10.";
								MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
								MessageBox.SetText(Message1, null);
								MessageBox.ClickAnywhereToClose = true;
								MessageBox.AllowCancel = false;
								MessageBox.Show();

								break;
							case 9:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42449));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownVillage)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for merchants.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 10:
								if (Religious1.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 92);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious2.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 88);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious3.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 98);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious4.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 82);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious5.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 84);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious6.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 94);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious7.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 36);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								if (Religious8.Equals(KnightOrderName, StringComparison.OrdinalIgnoreCase))
								{
									Guild = GManager.JoinGuild((FactionFile.GuildGroups)17, 106);
									GManager.AddMembership((FactionFile.GuildGroups)17, Guild);
								}

								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;

								RandoF = Fortuna.Next(0, 10);
								RandoF *= FLevel;
								RandoI = (int)RandoF;

								Guild.Rank = RandoI;
								Guild.UpdateRank(Player);

								Message1 = "You are a member of a religious " +
								"order. " + string.Format("{0}", KnightOrderName) +
								". Your rank is " + string.Format("{0}", RandoI) + "/10.";
								MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
								MessageBox.SetText(Message1, null);
								MessageBox.ClickAnywhereToClose = true;
								MessageBox.AllowCancel = false;
								MessageBox.Show();

								break;
							case 11:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42448));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownHamlet)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for pawners.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 12:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42453));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for scholars.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 13:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42447));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for tailors.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 14:
								Guild = GManager.JoinGuild((FactionFile.GuildGroups)4, 42);
								GManager.AddMembership((FactionFile.GuildGroups)4, Guild);

								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;

								RandoF = Fortuna.Next(0, 10);
								RandoF *= FLevel;
								RandoI = (int)RandoF;

								Guild.Rank = RandoI;
								Guild.UpdateRank(Player);

								Message1 = "You are a member of the Thieves " +
								"guild. Your rank is " + string.Format("{0}", RandoI) + "/10.";
								MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
								MessageBox.SetText(Message1, null);
								MessageBox.ClickAnywhereToClose = true;
								MessageBox.AllowCancel = false;
								MessageBox.Show();

								break;
							case 15:
								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;
								ILevel = (int)FLevel;

								RandoI = Fortuna.Next(0, 3);
								for (Index = 0; Index < RandoI; Index++)
								{
									AItem = ItemBuilder.CreateRandomWeapon(ILevel);
									Backpack.AddItem(AItem);
								}

								RandoI = Fortuna.Next(0, 3);
								for (Index = 0; Index < RandoI; Index++)
								{
									AItem = ItemBuilder.CreateRandomArmor(ILevel, AGender, ARace);
									Backpack.AddItem(AItem);
								}

								RandoI = Fortuna.Next(0, 6);
								for (Index = 0; Index < RandoI; Index++)
								{
									AItem = ItemBuilder.CreateRandomMagicItem(ILevel, AGender, ARace);
									Backpack.AddItem(AItem);
								}

								AItem = ItemBuilder.CreateRandomBook();
								BItem = ItemBuilder.CreateRandomClothing(AGender, ARace);
								CItem = ItemBuilder.CreateRandomClassicPotion();
								DItem = ItemBuilder.CreateRandomIngredient();
								EItem = ItemBuilder.CreateRandomReligiousItem();

								BItem.dyeColor = ItemBuilder.RandomClothingDye();

								Backpack.AddItem(AItem);
								Backpack.AddItem(BItem);
								Backpack.AddItem(CItem);
								Backpack.AddItem(DItem);
								Backpack.AddItem(EItem);

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownVillage)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for wanderers.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 16:
								Guild = GManager.JoinGuild((FactionFile.GuildGroups)22, 42);

								Book = ItemBuilder.CreateItem(ItemGroups.Weapons, 42454);
								ItemBuilder.ApplyWeaponMaterial(Book, (WeaponMaterialTypes)2);
								Backpack.AddItem(Book);

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.Coven)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for warlocks and witches.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 17:
								Guild = GManager.JoinGuild((FactionFile.GuildGroups)10, 42);
								GManager.AddMembership((FactionFile.GuildGroups)10, Guild);

								ILuck = Player.Stats.LiveLuck;
								FLuck = (float)ILuck / 100f;
								FLevel = ((((float)ExperienceYears * 32f) / 100f) + 1f) * FLuck;

								RandoF = Fortuna.Next(0, 10);
								RandoF *= FLevel;
								RandoI = (int)RandoF;

								Guild.Rank = RandoI;
								Guild.UpdateRank(Player);

								Message1 = "You are a member of the Mages " +
								"guild. Your rank is " + string.Format("{0}", RandoI) + "/10.";
								MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
								MessageBox.SetText(Message1, null);
								MessageBox.ClickAnywhereToClose = true;
								MessageBox.AllowCancel = false;
								MessageBox.Show();

								break;
							case 18:
								Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, 42450));

								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.TownCity)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for weaponsmiths.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							case 19:
								if (TeleportMe)
								{
									RandoI = Fortuna.Next(0, 71);
									ASRegion = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(RandoI);

									Locations = new List<int>();
									for (Index = 0; Index < ASRegion.LocationCount; Index++)
									{
										if (ASRegion.MapTable[Index].LocationType == DFRegion.LocationTypes.Graveyard)
										{
											Locations.Add(Index);
										}
									}

									RandoA = Fortuna.Next(0, (Locations.Count + 1));
									Location = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(RandoI, Locations[RandoA]);
									if (!Location.Loaded)
									{
										throw new System.Exception("Alternate Start - Could not locate a suitable site for the homeless.");
									}

									Pixel = MapsFile.LongitudeLatitudeToMapPixel(Location.MapTableData.Longitude, Location.MapTableData.Latitude);
									Position = MapsFile.MapPixelToWorldCoord(Pixel.X, Pixel.Y);
									GameManager.Instance.PlayerEnterExit.RespawnPlayer(Position.X, Position.Y, false, false);
								}

								break;
							default:
								break;
						}

						if (VampWolf == 1)
						{
							DaggerfallEntityBehaviour Behavior = GameManager.Instance.PlayerEntityBehaviour;
							EntityEffectManager VampirismManager = Behavior.GetComponent<EntityEffectManager>();
							EntityEffectBundle VampirismBundle = VampirismManager.CreateVampirismCurse();
							VampirismManager.AssignBundle(VampirismBundle, AssignBundleFlags.BypassSavingThrows);
						}
						else if (VampWolf == 2)
						{
							DaggerfallEntityBehaviour Behavior = GameManager.Instance.PlayerEntityBehaviour;
							EntityEffectManager LycanthropyManager = Behavior.GetComponent<EntityEffectManager>();
							EntityEffectBundle LycanthropyBundle = LycanthropyManager.CreateLycanthropyCurse();
							LycanthropyManager.AssignBundle(LycanthropyBundle, AssignBundleFlags.BypassSavingThrows);
						}

						GameStarted = 2;
					}
				}

				if (DrumPlayed == 1)
				{
					IStrength = Player.Stats.LiveStrength;
					FStrength = (float)IStrength / 100f;

					IAgility = Player.Stats.LiveAgility;
					FAgility = ((float)IAgility * 10800f) / 100f;

					ILuck = Player.Stats.LiveLuck;
					FLuck = ((float)ILuck / 100f) * FAgility;

					RandoF = ((float)ILuck / 100f) * DrumSkill;

					MinuteDayI = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.MinuteOfDay;
					DaggerfallDateTime Time = DaggerfallUnity.Instance.WorldTime.Now;

					if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
					{
						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle)
						{
							Message1 = "Members of the court look somewhat " +
							"bemused as you take out your drum and set it up.";

							if (MinuteDayI < 1020)
							{
								MinuteDayF = (float)MinuteDayI / 1020f;
							}
							else if (MinuteDayI > 1200)
							{
								MinuteDayF = 1200f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IEtiquette  = Player.Skills.GetLiveSkillValue(1);
							FEtiquette = (1f - ((float)IEtiquette / 100f)) * 100f;

							if (RandoF < FEtiquette)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Despite your best " +
								"efforts, the crowd is not pleased. " +
								"(Drum Skill " + string.Format("{0}/{1})", RandoF, FEtiquette);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 301);

								RandoF = (float)RandoI * MinuteDayF * FStrength;
								RandoI = (int)RandoF;

								MaxGoldF = (DrumSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								DrumSkill += 1f;
							}
						}

						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
						{
							Message1 = "Some particularly drunk people start " +
							"to chuckle when they see you take out your drum.";

							if (MinuteDayI < 1260)
							{
								MinuteDayF = (float)MinuteDayI / 1260f;
							}
							else if (MinuteDayI > 1440)
							{
								MinuteDayF = 1440f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Someone from in the crowd starts " +
								"hissing at you. Another angrily yells. " +
								"(Drum Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 101);

								RandoF = (float)RandoI * MinuteDayF * FStrength;
								RandoI = (int)RandoF;

								MaxGoldF = (DrumSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								DrumSkill += 1f;
							}
						}
					}
					else
					{
						if (NPCCheck == 1)
						{
							Message1 = "Pulling out your drum, you notice a few " +
							"curious onlookers looking to see what you're doing.";

							if (MinuteDayI < 660)
							{
								MinuteDayF = (float)MinuteDayI / 660f;
							}
							else if (MinuteDayI > 780)
							{
								MinuteDayF = 780f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "However hard you try, you just " +
								"can't keep the interest of the crowd. " +
								"(Drum Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 201);

								RandoF = (float)RandoI * MinuteDayF * FStrength;
								RandoI = (int)RandoF;

								MaxGoldF = (DrumSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								DrumSkill += 1f;
							}

							NPCCheck = 0;
						}
					}

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message2, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message1, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					DrumPlayed = 0;
				}

				if (LutePlayed == 1)
				{
					IIntelligence = Player.Stats.LiveIntelligence;
					FIntelligence = (float)IIntelligence / 100f;

					IWillpower = Player.Stats.LiveWillpower;
					FWillpower = ((float)IWillpower * 10800f) / 100f;

					ILuck = Player.Stats.LiveLuck;
					FLuck = ((float)ILuck / 100f) * FWillpower;

					RandoF = ((float)ILuck / 100f) * LuteSkill;

					MinuteDayI = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.MinuteOfDay;
					DaggerfallDateTime Time = DaggerfallUnity.Instance.WorldTime.Now;

					if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
					{
						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle)
						{
							Message1 = "Plucking a few strings of the lute " +
							"seems to get the attention of the court.";


							if (MinuteDayI < 1020)
							{
								MinuteDayF = (float)MinuteDayI / 1020f;
							}
							else if (MinuteDayI > 1200)
							{
								MinuteDayF = 1200f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IEtiquette  = Player.Skills.GetLiveSkillValue(1);
							FEtiquette = (1f - ((float)IEtiquette / 100f)) * 100f;

							if (RandoF < FEtiquette)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Despite your best " +
								"efforts, the crowd is not pleased. " +
								"(Lute Skill " + string.Format("{0}/{1})", RandoF, FEtiquette);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 301);

								RandoF = (float)RandoI * MinuteDayF * FIntelligence;
								RandoI = (int)RandoF;

								MaxGoldF = (LuteSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								LuteSkill += 1f;
							}
						}

						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
						{
							Message1 = "Once you start strumming away with " +
							"your lute, you start to enjoy yourself.";


							if (MinuteDayI < 1260)
							{
								MinuteDayF = (float)MinuteDayI / 1260f;
							}
							else if (MinuteDayI > 1440)
							{
								MinuteDayF = 1440f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Someone from in the crowd starts " +
								"hissing at you. Another angrily yells. " +
								"(Lute Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 101);

								RandoF = (float)RandoI * MinuteDayF * FIntelligence;
								RandoI = (int)RandoF;

								MaxGoldF = (LuteSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								LuteSkill += 1f;
							}
						}
					}
					else
					{
						if (NPCCheck == 1)
						{
							Message1 = "Twangs from your lute flow out " +
							"through the city, past people walking by.";

							if (MinuteDayI < 660)
							{
								MinuteDayF = (float)MinuteDayI / 660f;
							}
							else if (MinuteDayI > 780)
							{
								MinuteDayF = 780f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "However hard you try, you just " +
								"can't keep the interest of the crowd. " +
								"(Lute Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 201);

								RandoF = (float)RandoI * MinuteDayF * FIntelligence;
								RandoI = (int)RandoF;

								MaxGoldF = (LuteSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								LuteSkill += 1f;
							}

							NPCCheck = 0;
						}
					}

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message2, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message1, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					LutePlayed = 0;
				}

				if (RecorderPlayed == 1)
				{
					IEndurance = Player.Stats.LiveEndurance;
					FEndurance = (float)IEndurance / 100f;

					IPersonality = Player.Stats.LivePersonality;
					FPersonality = ((float)IPersonality * 10800f) / 100f;

					ILuck = Player.Stats.LiveLuck;
					FLuck = ((float)ILuck / 100f) * FPersonality;

					RandoF = ((float)ILuck / 100f) * RecorderSkill;

					MinuteDayI = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.MinuteOfDay;
					DaggerfallDateTime Time = DaggerfallUnity.Instance.WorldTime.Now;

					if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
					{
						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle)
						{
							Message1 = "A shrill shriek from your recorder " +
							"causes the court to stop and stare at you.";


							if (MinuteDayI < 1020)
							{
								MinuteDayF = (float)MinuteDayI / 1020f;
							}
							else if (MinuteDayI > 1200)
							{
								MinuteDayF = 1200f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IEtiquette  = Player.Skills.GetLiveSkillValue(1);
							FEtiquette = (1f - ((float)IEtiquette / 100f)) * 100f;

							if (RandoF < FEtiquette)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Despite your best " +
								"efforts, the crowd is not pleased. " +
								"(Recorder Skill " + string.Format("{0}/{1})", RandoF, FEtiquette);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 301);

								RandoF = (float)RandoI * MinuteDayF * FEndurance;
								RandoI = (int)RandoF;

								MaxGoldF = (RecorderSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								RecorderSkill += 1f;
							}
						}

						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
						{
							Message1 = "Sonnets of distant lands and " +
							"forgotten pasts comes from your recorder.";


							if (MinuteDayI < 1260)
							{
								MinuteDayF = (float)MinuteDayI / 1260f;
							}
							else if (MinuteDayI > 1440)
							{
								MinuteDayF = 1440f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "Someone from in the crowd starts " +
								"hissing at you. Another angrily yells. " +
								"(Recorder Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 101);

								RandoF = (float)RandoI * MinuteDayF * FEndurance;
								RandoI = (int)RandoF;

								MaxGoldF = (RecorderSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								RecorderSkill += 1f;
							}
						}
					}
					else
					{
						if (NPCCheck == 1)
						{
							Message1 = "Cheerful piping from your recorder " +
							"causes several bystanders to stop in their tracks.";


							if (MinuteDayI < 660)
							{
								MinuteDayF = (float)MinuteDayI / 660f;
							}
							else if (MinuteDayI > 780)
							{
								MinuteDayF = 780f / (float)MinuteDayI;
							}
							else
							{
								MinuteDayF = 1f;
							}

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Message2 = "However hard you try, you just " +
								"can't keep the interest of the crowd. " +
								"(Recorder Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 201);

								RandoF = (float)RandoI * MinuteDayF * FEndurance;
								RandoI = (int)RandoF;

								MaxGoldF = (RecorderSkill / 100f) * RandoF;
								BonusGold = RandoI;
								MaxGoldI = (int)MaxGoldF + BonusGold;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You made " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								RecorderSkill += 1f;
							}

							NPCCheck = 0;
						}
					}

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message2, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message1, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					RecorderPlayed = 0;
				}

				if (MugUsed == 1)
				{
					Fortuna = new System.Random();
					RandoI = Fortuna.Next(0, 101);

					ILuck = Player.Stats.LiveLuck;
					FLuck = ((float)ILuck / 100f) * (float)RandoI;

					RandoF = ((float)ILuck / 100f) * BeggingSkill;

					DaggerfallDateTime Time = DaggerfallUnity.Instance.WorldTime.Now;

					if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
					{
						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeonCastle)
						{
							Message1 = "Going up to each member of the court, " +
							"you shove your mug under their faces and beg.";

							IEtiquette  = Player.Skills.GetLiveSkillValue(1);
							FEtiquette = (1f - ((float)IEtiquette / 100f)) * 100f;

							if (RandoF < FEtiquette)
							{
								Time.RaiseTime(FLuck);

								Crime = (PlayerEntity.Crimes)2; // "tresspassing"
								Player.CrimeCommitted = Crime;
								Player.SpawnCityGuards(true);

								Message2 = "Not only have you received no " +
								"money, but guards have been called on you. " +
								"(Begging Skill " + string.Format("{0}/{1})", RandoF, FEtiquette);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 4);

								MaxGoldI = RandoI;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You received " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								BeggingSkill += 1f;
							}
						}

						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideOpenShop)
						{
							Message1 = "Every customer you pass by, you show " +
							"them your mug, and your best puppy-dog eyes.";


							IStreetwise  = Player.Skills.GetLiveSkillValue(1);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Crime = (PlayerEntity.Crimes)2; // "tresspassing"
								Player.CrimeCommitted = Crime;
								Player.SpawnCityGuards(true);

								Message2 = "Furious at your actions, the shop " +
								"owner yells for guards to take you away. " +
								"(Begging Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Fortuna = new System.Random();
								RandoI = Fortuna.Next(0, 3);

								MaxGoldI = RandoI;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You received " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								BeggingSkill += 1f;
							}
						}

						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideTavern)
						{
							Message1 = "Flickering candlelight and the warmth " +
							"of the fire welcomes you from the elements.";


							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								Crime = (PlayerEntity.Crimes)2; // "tresspassing"
								Player.CrimeCommitted = Crime;
								Player.SpawnCityGuards(true);

								Message2 = "Pointing to the door, the bar " +
								"keep angrily tells you to leave at once. " +
								"(Begging Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								Time.RaiseTime(FLuck);
								Player.GoldPieces += 1;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You received " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								BeggingSkill += 1f;
							}
						}
					}
					else
					{
						if (NPCCheck == 1)
						{
							Message1 = "Several people walk past you as you " +
							"hold out your mug, going on with their lives.";

							Fortuna = new System.Random();

							IStreetwise  = Player.Skills.GetLiveSkillValue(2);
							FStreetwise = (1f - ((float)IStreetwise / 100f)) * 100f;

							if (RandoF < FStreetwise)
							{
								Time.RaiseTime(FLuck);

								RandoI = Fortuna.Next(0, 3);
								Player.DecreaseHealth(RandoI);

								Message2 = "Upset by your presence and " +
								"begging, an angry citizen punches you. " +
								"(Begging Skill " + string.Format("{0}/{1})", RandoF, FStreetwise);
							}
							else
							{
								RandoI = Fortuna.Next(0, 2);

								MaxGoldI = RandoI;

								Time.RaiseTime(FLuck);
								Player.GoldPieces += MaxGoldI;

								FLuck /= 3600f;
								Message2 = string.Format("{0}", FLuck) + " " +
								"hours have passed. You received " + string.Format("{0}", MaxGoldI) + " " +
								"gold.";

								BeggingSkill += 1f;
							}

							NPCCheck = 0;
						}
					}

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message2, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
					MessageBox.SetText(Message1, null);
					MessageBox.ClickAnywhereToClose = true;
					MessageBox.AllowCancel = false;
					MessageBox.Show();

					MugUsed = 0;
				}

				if (LawbookUsed == 1)
				{
					if (SpawnCount == 0)
					{
						Message1 = "While on duty, be on the lookout for any " +
						"criminals. They may appear any moment, so be vigilant.";
						MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
						MessageBox.SetText(Message1, null);
						MessageBox.ClickAnywhereToClose = true;
						MessageBox.AllowCancel = false;
						MessageBox.Show();

						SpawnCount += 1;
					}
					else if (SpawnCount < 12)
					{
						if (RandomCheck == 0)
						{
							Fortuna = new System.Random();
							RandomCheck = 1;
						}

						if (OldTime == 0)
						{
							OldTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.MinuteOfDay;
						}

						NewTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.MinuteOfDay;
						if (NewTime > OldTime)
						{
							NewTime -= OldTime;
							if (NewTime < 0)
							{
								NewTime = DaggerfallDateTime.HoursPerDay - (-1 * NewTime);
							}
							else if (NewTime >= 60)
							{
								RandoI = Fortuna.Next(0, 101);

								ILuck = Player.Stats.LiveLuck;
								FLuck = 1f - ((float)ILuck / 100f);

								RandoF = (float)RandoI * FLuck;
								RandoF = (RandoF * 12f) / 100f;

								if (RandoF < 8.33f) // 100% in 12 hours, ~8% in a hour
								{
									RandoI = Fortuna.Next(0, 5);

									switch (RandoI)
									{
										case 0:
											Mobile = MobileTypes.Assassin;
											break;
										case 1:
											Mobile = MobileTypes.Burglar;
											break;
										case 2:
											Mobile = MobileTypes.Rogue;
											break;
										case 3:
											Mobile = MobileTypes.Thief;
											break;
										case 4:
											Mobile = MobileTypes.Barbarian;
											break;
										default:
											Mobile = MobileTypes.Assassin;
											break;
									}

									GameObjectHelper.CreateFoeSpawner(false, Mobile, 1);
								}

								OldTime = 0;
								SpawnCount += 1;
							}
						}
					}
					else
					{
						Message1 = "After a hard day's shift protecting " +
						"the city, you finally recieve your compensation.";
						MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
						MessageBox.SetText(Message1, null);
						MessageBox.ClickAnywhereToClose = true;
						MessageBox.AllowCancel = false;
						MessageBox.Show();

						RandoI = Fortuna.Next(0, 101);
						RandoI *= (int)RandoF;
						Player.GoldPieces += RandoI;

						DaggerfallUI.AddHUDText(string.Format("{0} gold added", RandoI));

						LawbookUsed = 0;
					}
				}

				if (ShelfUsed == 1)
				{
					if (ShelfExists == 0)
					{
						if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
						{
							if (HasItems == 0)
							{
								Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(42446, 0, null);
								ShelfName = Bookshelf.ToString();
							}
							else
							{
								Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(TextureIndex, 0, null);
								ShelfName = Bookshelf.ToString();
							}

							//  - https://github.com/Ralzar81/Climates-Calories/blob/master/Climates%20%26%20Calories/Camping.cs
							GameObject player = GameManager.Instance.PlayerObject;
							ShelfPosition = player.transform.position + (player.transform.forward * 2);

							RaycastHit hit;
							Ray ray = new Ray(ShelfPosition, Vector3.down);
							if (Physics.Raycast(ray, out hit, 10))
							{
								ShelfPosition = hit.point + (Vector3.down * -1.28f);
								ShelfRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
							}

							Bookshelf.transform.SetPositionAndRotation(ShelfPosition, ShelfRotation);
							Bookshelf.SetActive(true);
							// -------------------------------------------------

							ShelfExists = 1;
						}
						else
						{
							DaggerfallUI.AddHUDText(string.Format("You cannot place the shelf here"));

							Backpack = Player.Items;
							Backpack.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems2, TemplateIndex));
						}
					}

					ShelfUsed = 0;
				}

				if (ShelfExists == 1)
				{
					if (OldTime == 0)
					{
						OldTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
					}

					NewTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
					if (NewTime != OldTime)
					{
						NewTime -= OldTime;
						if (NewTime < 0)
						{
							NewTime = DaggerfallDateTime.DaysPerYear - (-1 * NewTime);
						}

						for (DaysPassed = 0; DaysPassed < NewTime; DaysPassed++)
						{
							Fortuna = new System.Random();

							RandoI = Fortuna.Next(0, Shelf.Count); // find how many items sold
							FLuck = (float)Player.Stats.LiveLuck / 100f;
							RandoF = (float)RandoI * FLuck;
							RandoI = (int)RandoF;

							if (RandoI > 0)
							{
								BookCount = (Shelf.Count / RandoI) + 1;
								for (Index = 0; Index < Shelf.Count; Index += BookCount)
								{
									Book = Shelf[Index];
									BookData = Data[Index];
									ShelfValue += BookData.value1;

									Shelf.RemoveAt(Index);
									Data.RemoveAt(Index);
								}
							}

							RandoI = Fortuna.Next(0, Shelf.Count); // find how many items stolen
							FLuck = 1f - ((float)Player.Stats.LiveLuck / 100f);
							RandoF = (float)RandoI * FLuck;
							RandoI = (int)RandoF;

							if (RandoI > 0)
							{
								BookCount = (Shelf.Count / RandoI) + 1;
								for (Index = 0; Index < Shelf.Count; Index += BookCount)
								{
									Book = Shelf[Index];
									BookData = Data[Index];
									ShelfValue += BookData.value1;
									Shelf.RemoveAt(Index);
									Data.RemoveAt(Index);
								}
							}

							RandoI = Fortuna.Next(0, Shelf.Count);
							FLuck = 1f - (float)Player.Stats.LiveLuck / 100f;
							RandoF = (float)RandoI * FLuck;
							RandoI = (int)RandoF;

							if (RandoI > 0)
							{
								BookCount = Shelf.Count / RandoI + 1;
								for (Index = 0; Index < Shelf.Count; Index += BookCount)
								{
									Shelf.RemoveAt(Index);
									Data.RemoveAt(Index);
								}
							}
						}

						OldTime = 0;
					}
				}

				if (HasInited == 1)
				{
					if (ShelfExists == 1)
					{
						if (HasItems == 0)
						{
							Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(42446, 0, null);
							Bookshelf.transform.SetPositionAndRotation(ShelfPosition, ShelfRotation);
						}
						else
						{
							Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(TextureIndex, 0, null);
							Bookshelf.transform.SetPositionAndRotation(ShelfPosition, ShelfRotation);
						}

						Bookshelf.SetActive(true);
					}

					HasInited = 2;
				}
			}
		}

		public static void InitMod()
		{
			Debug.Log("Begin mod init: Alternate Start");

			// - Register custom items -----------------------------------------
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42440, ItemGroups.UselessItems2, typeof(DrumItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42441, ItemGroups.UselessItems2, typeof(LuteItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42442, ItemGroups.UselessItems2, typeof(RecorderItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42443, ItemGroups.UselessItems2, typeof(MugItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42444, ItemGroups.Books, typeof(LawbookItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42445, ItemGroups.UselessItems2, typeof(LibrarianItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42446, ItemGroups.UselessItems2, typeof(AlchemistItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42447, ItemGroups.UselessItems2, typeof(TailorItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42448, ItemGroups.UselessItems2, typeof(PawnerItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42449, ItemGroups.UselessItems2, typeof(MerchantItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42450, ItemGroups.UselessItems2, typeof(WeaponsmithItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42451, ItemGroups.UselessItems2, typeof(ArmorerItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42452, ItemGroups.UselessItems2, typeof(JewellerItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42453, ItemGroups.UselessItems2, typeof(ScholarItem));
			DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(42454, ItemGroups.Weapons, typeof(StaffWeapon));
			// -----------------------------------------------------------------

			// - Register custom activations -----------------------------------
			PlayerActivate.RegisterCustomActivation(ASMod, 42446, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42447, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42448, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42449, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42450, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42451, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42452, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42453, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42454, 0, OnShelfClicked);
			PlayerActivate.RegisterCustomActivation(ASMod, 42455, 0, OnShelfClicked);
			// -----------------------------------------------------------------

			// - Event handlers ------------------------------------------------
			DaggerfallAction.OnTeleportAction += ASOnTeleport;
			PopulationManager.OnMobileNPCCreate += ASNPCCreated;
			SaveLoadManager.OnLoad += ASOnLoad;
			StartGameBehaviour.OnStartGame += ASOnStartGame;
			// -----------------------------------------------------------------

			Backpack = new ItemCollection();

			GameBehaviour = GameObject.FindObjectOfType<StartGameBehaviour>();

			GManager = GameManager.Instance.GuildManager;

			HasInited = 1;

			Player = GameManager.Instance.PlayerEntity;
			AGender = Player.Gender;
			ARace = Player.Race;

			Settings = ASMod.GetSettings();

			Debug.Log("Finished mod init: Alternate Start");
		}

		public static void ASOnTeleport(GameObject triggerObj, GameObject nextObj)
		{
			LawbookUsed = 0;
			RandomCheck = 0;
			SpawnCount = 0;

			NPCCheck = 0;
		}

		public static void ASNPCCreated(PopulationManager.PoolItem poolItem)
		{
			if (NPCCheck == 0)
			{
				NPCCheck = 1;
			}
		}

		public static void ASOnLoad(SaveData_v1 saveData)
		{
			if (ShelfExists == 0)
			{
				if (Bookshelf != null)
				{
					UnityEngine.Object.Destroy(Bookshelf);
					Bookshelf = null;
					ShelfName = "";
				}
			}
		}

		public static void ASOnStartGame(object sender, EventArgs e)
		{
			if (GameBehaviour.StartMethod == StartGameBehaviour.StartMethods.NewCharacter)
			{
				GameStarted = 1;
			}
		}

		public static void OnShelfClicked(RaycastHit hit)
		{
			if (hit.transform.gameObject.GetInstanceID() == Bookshelf.GetInstanceID())
			{
				Message1 = "Do you want to add an item?";
				MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
				MessageBox.SetText(Message1, null);
				MessageBox.ClickAnywhereToClose = true;
				MessageBox.AllowCancel = false;
				MessageBox.AddButton(Yes);
				MessageBox.AddButton(No);
				MessageBox.AddButton(Cancel);
				MessageBox.OnButtonClick += AddButtonsClicked;
				MessageBox.Show();
			}
		}

		public static void AddButtonsClicked(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
		{
			if (messageBoxButton == Yes)
			{
				sender.CloseWindow();
				AddBook();
			}
			else if (messageBoxButton == No)
			{
				sender.CloseWindow();

				Message1 = "Do you want to remove an item?";
				MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
				MessageBox.SetText(Message1, null);
				MessageBox.ClickAnywhereToClose = true;
				MessageBox.AllowCancel = false;
				MessageBox.AddButton(Yes);
				MessageBox.AddButton(No);
				MessageBox.AddButton(Cancel);
				MessageBox.OnButtonClick += RemoveButtonsClicked;
				MessageBox.Show();
			}
			else
			{
				sender.CloseWindow();
			}
		}

		public static void RemoveButtonsClicked(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
		{
			if (messageBoxButton == Yes)
			{
				sender.CloseWindow();
				RemoveBook();
			}
			else if (messageBoxButton == No)
			{
				sender.CloseWindow();

				Message1 = "Do you want to inspect an item (only valid for books)?";
				MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
				MessageBox.SetText(Message1, null);
				MessageBox.ClickAnywhereToClose = true;
				MessageBox.AllowCancel = false;
				MessageBox.AddButton(Yes);
				MessageBox.AddButton(No);
				MessageBox.AddButton(Cancel);
				MessageBox.OnButtonClick += ReadButtonsClicked;
				MessageBox.Show();
			}
			else
			{
				sender.CloseWindow();
			}
		}

		public static void ReadButtonsClicked(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
		{
			if (messageBoxButton == Yes)
			{
				sender.CloseWindow();
				ReadBook();
			}
			else if (messageBoxButton == No)
			{
				sender.CloseWindow();

				if (Occupation == 7)
				{
					Message1 = "Do you want to collect any donations?";
				}
				else
				{
					Message1 = "Do you want to collect your earnings?";
				}

				MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
				MessageBox.SetText(Message1, null);
				MessageBox.ClickAnywhereToClose = true;
				MessageBox.AllowCancel = false;
				MessageBox.AddButton(Yes);
				MessageBox.AddButton(No);
				MessageBox.AddButton(Cancel);
				MessageBox.OnButtonClick += MoneyButtonsClicked;
				MessageBox.Show();
			}
			else
			{
				sender.CloseWindow();
			}
		}

		public static void MoneyButtonsClicked(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
		{
			if (messageBoxButton == Yes)
			{
				sender.CloseWindow();
				CollectMoney();
			}
			else if (messageBoxButton == No)
			{
				sender.CloseWindow();

				Message1 = "Do you want to pack up the shelf?";
				MessageBox = new DaggerfallMessageBox(DaggerfallUI.UIManager, null, true, -1);
				MessageBox.SetText(Message1, null);
				MessageBox.ClickAnywhereToClose = true;
				MessageBox.AllowCancel = false;
				MessageBox.AddButton(Yes);
				MessageBox.AddButton(No);
				MessageBox.OnButtonClick += PackButtonsClicked;
				MessageBox.Show();
			}
			else
			{
				sender.CloseWindow();
			}
		}

		public static void PackButtonsClicked(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
		{
			if (messageBoxButton == Yes)
			{
				sender.CloseWindow();
				PackShelf();
			}
			else
			{
				sender.CloseWindow();
			}
		}

		// - DaggerfallBookshelf.cs --------------------------------------------
		public static void AddBook()
		{
			IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
			DaggerfallListPickerWindow bookPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
			bookPicker.OnItemPicked += BookShelfAdd_OnItemPicked;

			Backpack = Player.Items;
			for (Index = 0; Index < Backpack.Count; Index++)
			{
				Book = Backpack.GetItem(Index);

				BookData = Book.GetSaveData();
				if (BookData.itemGroup == ItemGroups.Books)
				{
					int bookMessage = BookData.message;
					string bookShort = BookData.shortName;
					string bookLong = DaggerfallUnity.Instance.ItemHelper.GetBookTitle(bookMessage, bookShort);

					bookPicker.ListBox.AddItem(bookLong);
				}
				else
				{
					string bookShort = BookData.shortName;
					bookPicker.ListBox.AddItem(bookShort);
				}
			}

			uiManager.PushWindow(bookPicker);
		}

		public static void BookShelfAdd_OnItemPicked(int index, string bookName)
		{
			Backpack = Player.Items;
			Book = Backpack.GetItem(index);
			BookData = Book.GetSaveData();

			switch(Occupation)
			{
				case 0:
					switch(BookData.itemGroup)
					{
						case ItemGroups.PlantIngredients1:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.PlantIngredients2:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.CreatureIngredients1:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.CreatureIngredients2:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.CreatureIngredients3:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MiscellaneousIngredients1:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MetalIngredients:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MiscellaneousIngredients2:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						default:
							break;
					}

					break;
				case 1:
					if (BookData.itemGroup == ItemGroups.Armor)
					{
						Backpack.RemoveItem(Book);
						Shelf.Add(Book);
						Data.Add(BookData);
					}

					break;
				case 5:
					switch(BookData.itemGroup)
					{
						case ItemGroups.Gems:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.Jewellery:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						default:
							break;
					}

					break;
				case 7:
					if (BookData.itemGroup == ItemGroups.Books)
					{
						Backpack.RemoveItem(Book);
						Shelf.Add(Book);
						Data.Add(BookData);
					}

					break;
				case 9:
					switch(BookData.itemGroup)
					{
						case ItemGroups.UselessItems1:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MagicItems:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.UselessItems2:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.Paintings:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MiscItems:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						default:
							break;
					}

					break;
				case 11:
					switch(BookData.itemGroup)
					{
						case ItemGroups.UselessItems1:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.UselessItems2:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.ReligiousItems:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.Paintings:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.MiscItems:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						default:
							break;
					}

					break;
				case 12:
					if (BookData.itemGroup == ItemGroups.Books)
					{
						Backpack.RemoveItem(Book);
						Shelf.Add(Book);
						Data.Add(BookData);
					}

					break;
				case 13:
					switch(BookData.itemGroup)
					{
						case ItemGroups.MensClothing:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						case ItemGroups.WomensClothing:
							Backpack.RemoveItem(Book);
							Shelf.Add(Book);
							Data.Add(BookData);

							break;
						default:
							break;
					}

					break;
				case 18:
					if (BookData.itemGroup == ItemGroups.Weapons)
					{
						Backpack.RemoveItem(Book);
						Shelf.Add(Book);
						Data.Add(BookData);
					}

					break;
				default:
					break;
			}

			if (Shelf.Count == 1)
			{
				UnityEngine.Object.Destroy(Bookshelf);

				Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(TextureIndex, 0, null);
				Bookshelf.transform.SetPositionAndRotation(ShelfPosition, ShelfRotation);
				Bookshelf.SetActive(true);

				ShelfName = Bookshelf.ToString();

				HasItems = 1;
			}

			DaggerfallUI.UIManager.PopWindow();
		}

		public static void RemoveBook()
		{
			IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
			DaggerfallListPickerWindow bookPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
			bookPicker.OnItemPicked += BookShelfRemove_OnItemPicked;

			for (Index = 0; Index < Shelf.Count; Index++)
			{
				Book = Shelf[Index];

				BookData = Data[Index];
				string bookShort = BookData.shortName;

				if (BookData.itemGroup == ItemGroups.Books)
				{
					int bookMessage = BookData.message;
					string bookLong = DaggerfallUnity.Instance.ItemHelper.GetBookTitle(bookMessage, bookShort);

					bookPicker.ListBox.AddItem(bookLong);
				}
				else
				{
					bookPicker.ListBox.AddItem(bookShort);
				}
			}

			uiManager.PushWindow(bookPicker);
		}

		public static void BookShelfRemove_OnItemPicked(int index, string bookName)
		{
			if (Shelf.Count == 1)
			{
				UnityEngine.Object.Destroy(Bookshelf);

				Bookshelf = GameObjectHelper.CreateDaggerfallBillboardGameObject(42446, 0, null);
				Bookshelf.transform.SetPositionAndRotation(ShelfPosition, ShelfRotation);
				Bookshelf.SetActive(true);

				ShelfName = Bookshelf.ToString();

				HasItems = 0;
			}

			Book = Shelf[index];
			BookData = Data[index];

			Shelf.RemoveAt(index);
			Data.RemoveAt(index);

			Book = new DaggerfallUnityItem(BookData);

			Backpack = Player.Items;
			Backpack.AddItem(Book);

			DaggerfallUI.UIManager.PopWindow();
		}

		public static void ReadBook()
		{
			IUserInterfaceManager uiManager = DaggerfallUI.UIManager;
			DaggerfallListPickerWindow bookPicker = new DaggerfallListPickerWindow(uiManager, uiManager.TopWindow);
			bookPicker.OnItemPicked += BookShelfRead_OnItemPicked;

			for (Index = 0; Index < Shelf.Count; Index++)
			{
				Book = Shelf[Index];

				BookData = Data[Index];
				string bookShort = BookData.shortName;

				if (BookData.itemGroup == ItemGroups.Books)
				{
					int bookMessage = BookData.message;
					string bookLong = DaggerfallUnity.Instance.ItemHelper.GetBookTitle(bookMessage, bookShort);

					bookPicker.ListBox.AddItem(bookLong);
				}
				else
				{
					bookPicker.ListBox.AddItem(bookShort);
				}
			}

			uiManager.PushWindow(bookPicker);
		}

		public static void BookShelfRead_OnItemPicked(int index, string bookName)
		{
			DaggerfallUI.Instance.BookReaderWindow.OpenBook(Shelf[index]);
			DaggerfallUI.UIManager.PopWindow();
			DaggerfallUI.PostMessage(DaggerfallUIMessages.dfuiOpenBookReaderWindow);
		}
		// ---------------------------------------------------------------------

		public static void CollectMoney()
		{
			Player.GoldPieces += ShelfValue;
			DaggerfallUI.AddHUDText(string.Format("{0} gold added", ShelfValue));

			ShelfValue = 0;
		}

		public static void PackShelf()
		{
			UnityEngine.Object.Destroy(Bookshelf);
			Bookshelf = null;
			ShelfName = "";

			Book = ItemBuilder.CreateItem(ItemGroups.UselessItems2, TemplateIndex);

			Backpack = Player.Items;
			Backpack.AddItem(Book);

			ShelfExists = 0;
		}

		public Type SaveDataType
		{
			get { return typeof(AlternateData); }
		}

		public object NewSaveData()
		{
			return new AlternateData
			{
				occupation = Occupation,
				drumSkill = DrumSkill,
				luteSkill = LuteSkill,
				recorderSkill = RecorderSkill,
				beggingSkill = BeggingSkill,
				lawbookUsed = LawbookUsed,
				spawnCount = SpawnCount,
				hasItems = HasItems,
				shelfExists = ShelfExists,
				shelfValue = ShelfValue,
				shelfName = ShelfName,
				shelf = Shelf,
				data = Data,
				shelfPosition = ShelfPosition,
				shelfRotation = ShelfRotation,
				templateIndex = TemplateIndex,
				textureIndex = TextureIndex,
				oldTime = OldTime
			};
		}

		public object GetSaveData()
		{
			return new AlternateData
			{
				occupation = Occupation,
				drumSkill = DrumSkill,
				luteSkill = LuteSkill,
				recorderSkill = RecorderSkill,
				beggingSkill = BeggingSkill,
				lawbookUsed = LawbookUsed,
				spawnCount = SpawnCount,
				hasItems = HasItems,
				shelfExists = ShelfExists,
				shelfValue = ShelfValue,
				shelfName = ShelfName,
				shelf = Shelf,
				data = Data,
				shelfPosition = ShelfPosition,
				shelfRotation = ShelfRotation,
				templateIndex = TemplateIndex,
				textureIndex = TextureIndex,
				oldTime = OldTime
			};
		}

		public void RestoreSaveData(object saveData)
		{
			var myModSaveData = (AlternateData)saveData;

			Occupation = myModSaveData.occupation;
			DrumSkill = myModSaveData.drumSkill;
			LuteSkill = myModSaveData.luteSkill;
			RecorderSkill = myModSaveData.recorderSkill;
			BeggingSkill = myModSaveData.beggingSkill;
			LawbookUsed = myModSaveData.lawbookUsed;
			SpawnCount = myModSaveData.spawnCount;
			HasItems = myModSaveData.hasItems;
			ShelfExists = myModSaveData.shelfExists;
			ShelfValue = myModSaveData.shelfValue;
			ShelfName = myModSaveData.shelfName;
			Shelf = myModSaveData.shelf;
			Data = myModSaveData.data;
			ShelfPosition = myModSaveData.shelfPosition;
			ShelfRotation = myModSaveData.shelfRotation;
			TemplateIndex = myModSaveData.templateIndex;
			TextureIndex = myModSaveData.textureIndex;
			OldTime = myModSaveData.oldTime;
		}
	}

	public class DrumItem : DaggerfallUnityItem
	{
		public DrumItem() : base(ItemGroups.UselessItems2, 42440){}

		public override bool UseItem(ItemCollection collection)
		{
			this.LowerCondition(1, AlternateStartMod.Player, collection);

			AlternateStartMod.DrumPlayed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(DrumItem).ToString();

			return data;
		}
	}

	public class LuteItem : DaggerfallUnityItem
	{
		public LuteItem() : base(ItemGroups.UselessItems2, 42441){}

		public override bool UseItem(ItemCollection collection)
		{
			this.LowerCondition(1, AlternateStartMod.Player, collection);

			AlternateStartMod.LutePlayed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(LuteItem).ToString();

			return data;
		}
	}

	public class RecorderItem : DaggerfallUnityItem
	{
		public RecorderItem() : base(ItemGroups.UselessItems2, 42442){}

		public override bool UseItem(ItemCollection collection)
		{
			this.LowerCondition(1, AlternateStartMod.Player, collection);

			AlternateStartMod.RecorderPlayed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(RecorderItem).ToString();

			return data;
		}
	}

	public class MugItem : DaggerfallUnityItem
	{
		public MugItem() : base(ItemGroups.UselessItems2, 42443){}

		public override bool UseItem(ItemCollection collection)
		{
			this.LowerCondition(1, AlternateStartMod.Player, collection);

			AlternateStartMod.MugUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(MugItem).ToString();

			return data;
		}
	}

	public class LawbookItem : DaggerfallUnityItem
	{
		public LawbookItem() : base(ItemGroups.UselessItems2, 42444){}

		public override bool UseItem(ItemCollection collection)
		{
			this.LowerCondition(1, AlternateStartMod.Player, collection);

			AlternateStartMod.LawbookUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(LawbookItem).ToString();

			return data;
		}
	}

	public class LibrarianItem : DaggerfallUnityItem
	{
		public LibrarianItem() : base(ItemGroups.UselessItems2, 42445){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42445;
			AlternateStartMod.TextureIndex = 42447;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(LibrarianItem).ToString();

			return data;
		}
	}

	public class AlchemistItem : DaggerfallUnityItem
	{
		public AlchemistItem() : base(ItemGroups.UselessItems2, 42446){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42446;
			AlternateStartMod.TextureIndex = 42448;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(AlchemistItem).ToString();

			return data;
		}
	}

	public class TailorItem : DaggerfallUnityItem
	{
		public TailorItem() : base(ItemGroups.UselessItems2, 42447){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42447;
			AlternateStartMod.TextureIndex = 42449;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(TailorItem).ToString();

			return data;
		}
	}

	public class PawnerItem : DaggerfallUnityItem
	{
		public PawnerItem() : base(ItemGroups.UselessItems2, 42448){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42448;
			AlternateStartMod.TextureIndex = 42450;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(PawnerItem).ToString();

			return data;
		}
	}

	public class MerchantItem : DaggerfallUnityItem
	{
		public MerchantItem() : base(ItemGroups.UselessItems2, 42449){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42449;
			AlternateStartMod.TextureIndex = 42451;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(MerchantItem).ToString();

			return data;
		}
	}

	public class WeaponsmithItem : DaggerfallUnityItem
	{
		public WeaponsmithItem() : base(ItemGroups.UselessItems2, 42450){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42450;
			AlternateStartMod.TextureIndex = 42452;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(WeaponsmithItem).ToString();

			return data;
		}
	}

	public class ArmorerItem : DaggerfallUnityItem
	{
		public ArmorerItem() : base(ItemGroups.UselessItems2, 42451){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42451;
			AlternateStartMod.TextureIndex = 42453;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(ArmorerItem).ToString();

			return data;
		}
	}

	public class JewellerItem : DaggerfallUnityItem
	{
		public JewellerItem() : base(ItemGroups.UselessItems2, 42452){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42452;
			AlternateStartMod.TextureIndex = 42454;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(JewellerItem).ToString();

			return data;
		}
	}

	public class ScholarItem : DaggerfallUnityItem
	{
		public ScholarItem() : base(ItemGroups.UselessItems2, 42453){}

		public override bool UseItem(ItemCollection collection)
		{
			AlternateStartMod.TemplateIndex = 42453;
			AlternateStartMod.TextureIndex = 42455;

			if (AlternateStartMod.ShelfExists == 0)
			{
				AlternateStartMod.Occupation = 0;
			}

			collection.RemoveItem(this);
			AlternateStartMod.ShelfUsed = 1;
			return true;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(ScholarItem).ToString();

			return data;
		}
	}

	public class StaffWeapon : DaggerfallUnityItem
	{
		public StaffWeapon() : base(ItemGroups.Weapons, 42454){}

		public override int InventoryTextureArchive
		{
			get
			{
				PlayerEntity Player = GameManager.Instance.PlayerEntity;
				Genders AGender = Player.Gender;
				if (AGender == Genders.Female)
				{
					return 42456;
				}
				else
				{
					return 42457;
				}
			}
		}

		public override int GroupIndex
		{
			get { return 2; }
		}

		public override SoundClips GetEquipSound()
		{
			return SoundClips.EquipStaff;
		}

		public override SoundClips GetSwingSound()
		{
			return SoundClips.SwingMediumPitch;
		}

		public override int GetWeaponSkillUsed()
		{
			return (int)DFCareer.ProficiencyFlags.BluntWeapons;
		}

		public override int GetBaseDamageMin()
		{
			return 1;
		}

		public override int GetBaseDamageMax()
		{
			return 8;
		}

		public override ItemHands GetItemHands()
		{
			return ItemHands.Both;
		}

		public override WeaponTypes GetWeaponType()
		{
			return IsEnchanted ? WeaponTypes.Staff_Magic : WeaponTypes.Staff;
		}

		public override ItemData_v1 GetSaveData()
		{
			ItemData_v1 data = base.GetSaveData();
			data.className = typeof(StaffWeapon).ToString();

			return data;
		}
	}

	[FullSerializer.fsObject("v1")]
	public class AlternateData
	{
		public int occupation { get; set; }
		public float drumSkill { get; set; }
		public float luteSkill { get; set; }
		public float recorderSkill { get; set; }
		public float beggingSkill { get; set; }
		public int lawbookUsed { get; set; }
		public int spawnCount { get; set; }
		public int hasItems { get; set; }
		public int shelfExists { get; set; }
		public int shelfValue { get; set; }
		public string shelfName { get; set; }
		public List<DaggerfallUnityItem> shelf { get; set; }
		public List<ItemData_v1> data { get; set; }
		public Vector3 shelfPosition { get; set; }
		public Quaternion shelfRotation { get; set; }
		public int templateIndex { get; set; }
		public int textureIndex { get; set; }
		public int oldTime { get; set; }
	}
}
