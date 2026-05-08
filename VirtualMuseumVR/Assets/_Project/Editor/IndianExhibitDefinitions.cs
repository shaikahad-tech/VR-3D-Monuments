// =============================================================================
// IndianExhibitDefinitions.cs — Pre-configured Exhibit Data for Indian Heritage
// Virtual Museum VR Project
// =============================================================================
// Editor utility to auto-create ExhibitData ScriptableObjects for 10 iconic
// Indian architectural heritage sites. Run via:
// Tools > Virtual Museum > Create Indian Exhibits
// =============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Editor
{
    public static class IndianExhibitDefinitions
    {
        private const string SAVE_PATH = "Assets/_Project/ScriptableObjects/Exhibits/India/";

        [MenuItem("Tools/Virtual Museum/Create Indian Exhibits", false, 20)]
        public static void CreateAllIndianExhibits()
        {
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets/_Project", "ScriptableObjects");
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Exhibits"))
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Exhibits");
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Exhibits/India"))
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects/Exhibits", "India");

            CreateTajMahal();
            CreateQutubMinar();
            CreateHawaMahal();
            CreateRedFort();
            CreateKhajurahoTemple();
            CreateKonarkSunTemple();
            CreateMysorepalace();
            CreateBrihadisvararTemple();
            CreateSanchiStupa();
            CreateHumayunsTomb();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("═══════════════════════════════════════════════════");
            Debug.Log("  Created 10 Indian Heritage Exhibit Data Assets!");
            Debug.Log($"  Location: {SAVE_PATH}");
            Debug.Log("═══════════════════════════════════════════════════");

            EditorUtility.DisplayDialog("Indian Exhibits Created",
                "Successfully created 10 ExhibitData assets for Indian heritage sites.\n\n" +
                $"Location: {SAVE_PATH}\n\n" +
                "Next: Import your photogrammetry models and assign them to pedestals.",
                "OK");
        }

        // =================================================================
        // 1. Taj Mahal
        // =================================================================
        private static void CreateTajMahal()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "taj_mahal";
            data.title = "Taj Mahal";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Taj Mahal is an ivory-white marble mausoleum on the right bank of " +
                "the river Yamuna in Agra, India. It was commissioned in 1632 by the " +
                "Mughal emperor Shah Jahan to house the tomb of his favourite wife, " +
                "Mumtaz Mahal. The tomb is the centrepiece of a 17-hectare complex, " +
                "which includes a mosque and a guest house, and is set in formal " +
                "gardens bounded on three sides by a crenellated wall. The Taj Mahal " +
                "was designated as a UNESCO World Heritage Site in 1983.";
            data.era = "17th Century (1632-1653)";
            data.location = "Agra, Uttar Pradesh";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 850;
            data.originalPolygonCount = 2500000;
            data.optimizedPolygonCount = 15000;
            data.texturePartCount = 8;
            data.maxTextureResolution = 8192;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.05f;
            data.inspectionScale = 0.15f;
            data.rotationSpeed = 30f;
            data.autoRotateOnPedestal = true;
            data.autoRotationSpeed = 3f;
            data.isGrabbable = true;
            data.isWalkThrough = true;
            data.spotlightColor = new Color(1f, 0.95f, 0.85f); // warm ivory
            data.spotlightIntensity = 2.5f;
            data.pedestalGlowColor = new Color(1f, 0.84f, 0f); // gold

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_TajMahal.asset");
        }

        // =================================================================
        // 2. Qutub Minar
        // =================================================================
        private static void CreateQutubMinar()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "qutub_minar";
            data.title = "Qutub Minar";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Qutub Minar is a minaret and victory tower that forms part of the " +
                "Qutb complex, a UNESCO World Heritage Site in the Mehrauli area of " +
                "South Delhi. Standing at 72.5 metres (238 ft), it is the tallest brick " +
                "minaret in the world. The tower has five distinct storeys, each marked " +
                "by a projecting balcony. The first three storeys are made of red " +
                "sandstone; the fourth and fifth storeys are of marble and sandstone.";
            data.era = "12th-13th Century (1199-1220)";
            data.location = "Mehrauli, Delhi";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 620;
            data.originalPolygonCount = 1800000;
            data.optimizedPolygonCount = 12000;
            data.texturePartCount = 5;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "RealityCapture";
            data.pedestalDisplayScale = 0.03f;
            data.inspectionScale = 0.1f;
            data.rotationSpeed = 40f;
            data.autoRotateOnPedestal = true;
            data.autoRotationSpeed = 4f;
            data.isGrabbable = true;
            data.spotlightColor = new Color(0.95f, 0.7f, 0.4f); // warm sandstone
            data.spotlightIntensity = 2.2f;
            data.pedestalGlowColor = new Color(0.9f, 0.5f, 0.2f); // terracotta

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_QutubMinar.asset");
        }

        // =================================================================
        // 3. Hawa Mahal (Palace of Winds)
        // =================================================================
        private static void CreateHawaMahal()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "hawa_mahal";
            data.title = "Hawa Mahal (Palace of Winds)";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "Hawa Mahal is a palace in Jaipur, built from red and pink sandstone. " +
                "Constructed in 1799 by Maharaja Sawai Pratap Singh, the palace has " +
                "953 small windows (jharokhas) decorated with intricate latticework. " +
                "The original purpose was to allow royal women to observe street life " +
                "and festivals without being seen. Its unique five-storey exterior is " +
                "akin to the honeycomb of a beehive. The structure is only one room " +
                "deep in many places.";
            data.era = "18th Century (1799)";
            data.location = "Jaipur, Rajasthan";
            data.country = "India";
            data.captureYear = 2022;
            data.acquisitionMethod = AcquisitionMethod.Ground;
            data.sourcePhotoCount = 480;
            data.originalPolygonCount = 1200000;
            data.optimizedPolygonCount = 12000;
            data.texturePartCount = 4;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.04f;
            data.inspectionScale = 0.12f;
            data.rotationSpeed = 35f;
            data.autoRotateOnPedestal = true;
            data.spotlightColor = new Color(1f, 0.6f, 0.5f); // pink sandstone
            data.spotlightIntensity = 2.3f;
            data.pedestalGlowColor = new Color(1f, 0.4f, 0.6f); // rose pink

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_HawaMahal.asset");
        }

        // =================================================================
        // 4. Red Fort (Lal Qila)
        // =================================================================
        private static void CreateRedFort()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "red_fort";
            data.title = "Red Fort (Lal Qila)";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Red Fort is a historic fort in Old Delhi that served as the main " +
                "residence of the Mughal Emperors for nearly 200 years (1648-1857). " +
                "Built by Emperor Shah Jahan, the fort spans 254.67 acres and is " +
                "constructed from red sandstone. It was designated a UNESCO World " +
                "Heritage Site in 2007. Every year on Indian Independence Day (15th " +
                "August), the Prime Minister hoists the national flag at its main gate.";
            data.era = "17th Century (1638-1648)";
            data.location = "Old Delhi, Delhi";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Aerial;
            data.sourcePhotoCount = 1200;
            data.originalPolygonCount = 3200000;
            data.optimizedPolygonCount = 15000;
            data.texturePartCount = 10;
            data.maxTextureResolution = 8192;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.02f;
            data.inspectionScale = 0.08f;
            data.rotationSpeed = 25f;
            data.autoRotateOnPedestal = true;
            data.isWalkThrough = true;
            data.spotlightColor = new Color(0.9f, 0.4f, 0.3f); // red sandstone
            data.spotlightIntensity = 2.0f;
            data.pedestalGlowColor = new Color(0.8f, 0.2f, 0.1f); // deep red

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_RedFort.asset");
        }

        // =================================================================
        // 5. Khajuraho Temple Complex
        // =================================================================
        private static void CreateKhajurahoTemple()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "khajuraho_temple";
            data.title = "Khajuraho Temple (Kandariya Mahadeva)";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Kandariya Mahadeva Temple is the largest and most ornate Hindu " +
                "temple in the Khajuraho group of monuments, a UNESCO World Heritage " +
                "Site. Built around 1030 CE by the Chandela dynasty, the temple is " +
                "dedicated to Lord Shiva. It stands 30.5 metres tall and features " +
                "over 800 sculptures covering its exterior, including intricate " +
                "depictions of celestial beings, gods, and geometric patterns.";
            data.era = "11th Century (c. 1030 CE)";
            data.location = "Khajuraho, Madhya Pradesh";
            data.country = "India";
            data.captureYear = 2022;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 750;
            data.originalPolygonCount = 2000000;
            data.optimizedPolygonCount = 14000;
            data.texturePartCount = 6;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "RealityCapture";
            data.pedestalDisplayScale = 0.06f;
            data.inspectionScale = 0.2f;
            data.rotationSpeed = 40f;
            data.autoRotateOnPedestal = true;
            data.spotlightColor = new Color(0.95f, 0.85f, 0.6f); // warm sandstone
            data.spotlightIntensity = 2.4f;
            data.pedestalGlowColor = new Color(0.9f, 0.7f, 0.3f); // amber

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_Khajuraho.asset");
        }

        // =================================================================
        // 6. Konark Sun Temple
        // =================================================================
        private static void CreateKonarkSunTemple()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "konark_sun_temple";
            data.title = "Konark Sun Temple";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Konark Sun Temple is a 13th-century CE temple dedicated to the " +
                "Hindu Sun God Surya. Shaped as a colossal chariot with twelve pairs " +
                "of elaborately carved stone wheels drawn by seven horses, it is a " +
                "UNESCO World Heritage Site. The temple was built by King Narasimha " +
                "Deva I of the Eastern Ganga dynasty around 1250 CE. Known as the " +
                "'Black Pagoda' by European sailors, its intricately carved wheels " +
                "serve as sundials.";
            data.era = "13th Century (c. 1250 CE)";
            data.location = "Konark, Odisha";
            data.country = "India";
            data.captureYear = 2022;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 900;
            data.originalPolygonCount = 2800000;
            data.optimizedPolygonCount = 15000;
            data.texturePartCount = 8;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.04f;
            data.inspectionScale = 0.15f;
            data.rotationSpeed = 35f;
            data.autoRotateOnPedestal = true;
            data.isWalkThrough = true;
            data.spotlightColor = new Color(0.85f, 0.75f, 0.5f); // weathered stone
            data.spotlightIntensity = 2.2f;
            data.pedestalGlowColor = new Color(1f, 0.6f, 0f); // sun orange

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_KonarkSunTemple.asset");
        }

        // =================================================================
        // 7. Mysore Palace
        // =================================================================
        private static void CreateMysorepalace()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "mysore_palace";
            data.title = "Mysore Palace (Amba Vilas)";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Mysore Palace, also known as Amba Vilas Palace, is a historical " +
                "palace and royal residence in Mysuru, Karnataka. It is one of the " +
                "most famous tourist attractions in India, with over 6 million annual " +
                "visitors. The current structure was built between 1897 and 1912 in " +
                "the Indo-Saracenic style, blending Hindu, Muslim, Rajput, and Gothic " +
                "architectural elements. During the Dasara festival, the palace is " +
                "illuminated with nearly 100,000 light bulbs.";
            data.era = "Early 20th Century (1897-1912)";
            data.location = "Mysuru, Karnataka";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 680;
            data.originalPolygonCount = 1900000;
            data.optimizedPolygonCount = 13000;
            data.texturePartCount = 6;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.03f;
            data.inspectionScale = 0.1f;
            data.rotationSpeed = 30f;
            data.autoRotateOnPedestal = true;
            data.spotlightColor = new Color(1f, 0.9f, 0.7f); // warm gold
            data.spotlightIntensity = 2.5f;
            data.pedestalGlowColor = new Color(0.8f, 0.6f, 0.2f); // royal gold

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_MysorePalace.asset");
        }

        // =================================================================
        // 8. Brihadisvarar Temple (Big Temple)
        // =================================================================
        private static void CreateBrihadisvararTemple()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "brihadisvarar_temple";
            data.title = "Brihadisvarar Temple (Big Temple)";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Brihadisvarar Temple in Thanjavur is one of the largest Hindu " +
                "temples and an exemplary example of Dravidian architecture. Built " +
                "by Raja Raja Chola I between 1003 and 1010 CE, it is a UNESCO " +
                "World Heritage Site. The temple's vimana (tower) rises to 66 metres " +
                "(216 ft), making it one of the tallest in the world. The crowning " +
                "stone (shikhara) weighs approximately 80 tonnes and was moved to " +
                "the top using an inclined plane 6.44 km long.";
            data.era = "11th Century (1003-1010 CE)";
            data.location = "Thanjavur, Tamil Nadu";
            data.country = "India";
            data.captureYear = 2022;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 800;
            data.originalPolygonCount = 2200000;
            data.optimizedPolygonCount = 14000;
            data.texturePartCount = 7;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "RealityCapture";
            data.pedestalDisplayScale = 0.04f;
            data.inspectionScale = 0.12f;
            data.rotationSpeed = 35f;
            data.autoRotateOnPedestal = true;
            data.spotlightColor = new Color(0.85f, 0.75f, 0.55f); // granite
            data.spotlightIntensity = 2.3f;
            data.pedestalGlowColor = new Color(0.7f, 0.5f, 0.3f); // warm brown

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_BrihadisvararTemple.asset");
        }

        // =================================================================
        // 9. Sanchi Stupa
        // =================================================================
        private static void CreateSanchiStupa()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "sanchi_stupa";
            data.title = "Great Stupa at Sanchi";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "The Great Stupa at Sanchi is the oldest stone structure in India, " +
                "originally commissioned by Emperor Ashoka the Great in the 3rd " +
                "century BCE. It is a UNESCO World Heritage Site and one of the " +
                "most important Buddhist monuments in the world. The hemispherical " +
                "dome stands 16.5 metres high and 36.6 metres in diameter. The " +
                "four elaborately carved toranas (gateways) depict scenes from the " +
                "life of the Buddha and Jataka tales.";
            data.era = "3rd Century BCE - 1st Century CE";
            data.location = "Sanchi, Madhya Pradesh";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Aerial;
            data.sourcePhotoCount = 550;
            data.originalPolygonCount = 1500000;
            data.optimizedPolygonCount = 11000;
            data.texturePartCount = 4;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.06f;
            data.inspectionScale = 0.2f;
            data.rotationSpeed = 40f;
            data.autoRotateOnPedestal = true;
            data.spotlightColor = new Color(0.9f, 0.85f, 0.7f); // warm stone
            data.spotlightIntensity = 2.1f;
            data.pedestalGlowColor = new Color(0.6f, 0.5f, 0.3f); // earth tone

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_SanchiStupa.asset");
        }

        // =================================================================
        // 10. Humayun's Tomb
        // =================================================================
        private static void CreateHumayunsTomb()
        {
            var data = ScriptableObject.CreateInstance<ExhibitData>();
            data.exhibitId = "humayuns_tomb";
            data.title = "Humayun's Tomb";
            data.room = MuseumRoom.IndianArchitecture;
            data.description =
                "Humayun's Tomb is the tomb of the Mughal Emperor Humayun in Delhi. " +
                "Commissioned by his first wife Bega Begum in 1569-70, it was the " +
                "first garden-tomb on the Indian subcontinent and served as the " +
                "inspiration for the Taj Mahal. It is a UNESCO World Heritage Site. " +
                "The tomb is built of red sandstone with white marble accents and " +
                "stands in a complex of Mughal-era buildings within a geometrically " +
                "planned Charbagh garden, a Persian-style garden divided into four " +
                "quadrants by water channels.";
            data.era = "16th Century (1569-1570)";
            data.location = "Nizamuddin East, Delhi";
            data.country = "India";
            data.captureYear = 2023;
            data.acquisitionMethod = AcquisitionMethod.Combined;
            data.sourcePhotoCount = 720;
            data.originalPolygonCount = 2100000;
            data.optimizedPolygonCount = 13000;
            data.texturePartCount = 6;
            data.maxTextureResolution = 4096;
            data.reconstructionSoftware = "Agisoft Metashape";
            data.pedestalDisplayScale = 0.04f;
            data.inspectionScale = 0.12f;
            data.rotationSpeed = 30f;
            data.autoRotateOnPedestal = true;
            data.isWalkThrough = true;
            data.spotlightColor = new Color(0.95f, 0.75f, 0.5f); // sandstone + marble
            data.spotlightIntensity = 2.3f;
            data.pedestalGlowColor = new Color(0.9f, 0.3f, 0.2f); // Mughal red

            AssetDatabase.CreateAsset(data, $"{SAVE_PATH}Exhibit_HumayunsTomb.asset");
        }
    }
}
#endif
