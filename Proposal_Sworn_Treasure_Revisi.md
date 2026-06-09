PROPOSAL

Perancangan Game 2D "Sworn Treasure" dengan Mekanik Penggalian, Sistem Artefak Nusantara, dan Manajemen Energi

Disusun untuk memenuhi tugas Interaksi Manusia dan Komputer

Disusun oleh:
- Nabil Pratama Hidayat, 2491345074

PROGRAM STUDI TEKNOLOGI PERMAINAN
JURUSAN DESAIN
POLITEKNIK NEGERI MEDIA KREATIF
TAHUN 2026

KATA PENGANTAR

Puji syukur penulis panjatkan ke hadirat Tuhan Yang Maha Esa karena atas rahmat dan karunia-Nya proposal perancangan game "Sworn Treasure" dapat diselesaikan dengan baik.

Proposal ini disusun untuk memenuhi tugas akademik pada Program Studi Teknologi Permainan. Proposal ini memaparkan rancangan game 2D yang menggunakan mekanik penggalian berbasis grid, sistem manajemen energi atau Paw Durability, sistem point, Shop, Bag, serta sistem penemuan artefak Nusantara. Pemain mengendalikan karakter anjing yang menggali tanah untuk mengumpulkan point, menemukan item, dan mencari artefak asli Indonesia pada kedalaman tertentu.

Dalam penyusunan proposal ini, penulis berusaha menyusun konsep permainan, mekanik gameplay, antarmuka, sistem ekonomi, sistem artefak, serta rencana pengujian secara terstruktur berdasarkan perkembangan implementasi game terbaru. Semoga proposal ini dapat memberikan manfaat dan menjadi dasar yang kuat dalam proses pengembangan game Sworn Treasure.

Jakarta, 5 Juni 2026

Penulis

DAFTAR ISI

* KATA PENGANTAR
* BAB I PENDAHULUAN
* BAB II TINJAUAN PUSTAKA
* BAB III PERANCANGAN SISTEM
* BAB IV RENCANA IMPLEMENTASI DAN PENGUJIAN
* BAB V PENUTUP
* DAFTAR PUSTAKA

BAB I: PENDAHULUAN

1.1 Latar Belakang

Perkembangan industri game digital semakin pesat dan menawarkan berbagai variasi genre serta mekanik permainan. Salah satu pendekatan yang digemari pemain adalah eksplorasi bawah tanah dan penemuan harta karun. Mekanik penggalian yang dikombinasikan dengan manajemen sumber daya dapat memberikan tantangan yang ringan, mudah dipahami, tetapi tetap menarik untuk dimainkan berulang kali.

Game "Sworn Treasure" dirancang sebagai game 2D dengan sudut pandang side view dan sistem penggalian berbasis grid. Pemain mengendalikan karakter anjing yang dapat menggali ke bawah, kiri, dan kanan. Setiap aktivitas penggalian mengurangi Paw Durability atau energy. Pemain perlu mengatur energy, mengumpulkan point, membeli energy di Shop, serta menggunakan tombol Back to Top pada kondisi tertentu untuk kembali ke permukaan.

Keunikan game ini terletak pada sistem artefak Nusantara. Selain item dasar seperti gold, bone, dan energy, pemain dapat menemukan lima artefak asli Indonesia dengan rarity, chance, reward point, dan rentang kedalaman yang berbeda. Sistem ini membuat eksplorasi bawah tanah terasa bertahap karena artefak yang lebih langka hanya dapat ditemukan pada kedalaman tertentu.

1.2 Rumusan Masalah

Berdasarkan latar belakang tersebut, rumusan masalah dalam perancangan game ini adalah:

1. Bagaimana merancang game eksplorasi 2D "Sworn Treasure" dengan mekanik penggalian berbasis grid?
2. Bagaimana menerapkan sistem manajemen Paw Durability atau energy pada saat pemain melakukan penggalian?
3. Bagaimana mengimplementasikan sistem artefak Nusantara berdasarkan rarity, chance, reward point, dan rentang kedalaman tertentu?
4. Bagaimana merancang antarmuka Shop, Bag, HUD, dan alert agar informasi permainan mudah dipahami pemain?

1.3 Batasan Masalah

Agar pengembangan lebih terarah, batasan masalah yang ditetapkan adalah:

* Game dikembangkan dengan gaya visual 2D side view dan mekanik penggalian berbasis grid.
* Karakter utama adalah seekor anjing dengan pergerakan terbatas untuk menggali ke kanan, kiri, dan bawah.
* Mekanik utama berfokus pada penggalian, pengurangan Paw Durability atau energy, pengumpulan point, pembelian energy di Shop, dan penemuan artefak.
* Kapasitas energy maksimal karakter adalah 50.
* Area permainan menggunakan sistem kedalaman hingga 300 depth.
* Artefak dibagi menjadi lima rarity: Common, Uncommon, Rare, Ultra Rare, dan Mythical.
* Tombol Back to Top hanya dapat digunakan ketika energy habis atau pemain sudah melewati depth 50.

1.4 Tujuan Perancangan

Tujuan dari proposal ini adalah:

* Merancang dokumen dan sistem untuk game "Sworn Treasure" sebagai game eksplorasi penggalian dan pencarian artefak.
* Menerapkan mekanik Paw Durability, sistem point, Shop, Bag, Back to Top, dan sistem artefak Nusantara ke dalam core loop permainan.
* Menyusun antarmuka yang informatif agar pemain dapat memahami status point, energy, item shop, dan peluang penemuan artefak.
* Menyusun alur kerja terstruktur untuk tim pengembang.

1.5 Manfaat Perancangan

Manfaat dari perancangan game ini meliputi sarana hiburan kasual bagi pemain yang mengasah ketelitian, strategi eksplorasi, dan manajemen sumber daya. Dari sisi pengembangan, proyek ini menjadi wadah penerapan grid system, tilemap, state management, sistem probabilitas, UI/UX game, dan feedback visual pada game engine 2D.

BAB II: TINJAUAN PUSTAKA

2.1 Landasan Teori

2.1.1 Game Eksplorasi 2D

Game 2D merupakan permainan dengan aset grafis dua dimensi yang berfokus pada sumbu X dan Y. Eksplorasi bawah tanah yang membagi area menjadi sistem lapisan atau kedalaman memberikan ilusi progres linear kepada pemain saat menggali semakin dalam.

2.1.2 Grid Based Movement

Sistem grid membagi area permainan menjadi kotak-kotak koordinat yang terukur. Pemain bergerak dari satu kotak ke kotak lainnya secara presisi. Pendekatan ini memudahkan perhitungan posisi tile, interaksi penggalian, penempatan item, serta pembatasan pergerakan karakter.

2.1.3 Sistem Probabilitas Artefak dan Depth Range

Sistem artefak menggunakan probabilitas atau chance yang disesuaikan dengan kedalaman pemain. Setiap artefak memiliki rarity, reward point, rentang depth yang disarankan, dan chance kemunculan. Artefak tertentu tidak dapat muncul sebelum depth minimum agar progres permainan terasa bertahap.

2.1.4 Manajemen Sumber Daya

Manajemen sumber daya dalam game diterapkan melalui Paw Durability atau energy. Setiap aktivitas menggali mengurangi energy. Pemain perlu memperhatikan sisa energy, memutuskan kapan kembali ke permukaan, serta membeli energy di Shop menggunakan point yang dikumpulkan selama eksplorasi.

2.1.5 MDA Framework

Perancangan ini mengikuti MDA Framework (Mechanics, Dynamics, Aesthetics):

* Mechanics: Aturan penggalian berbasis grid, Paw Durability maksimal 50, sistem point, Shop untuk membeli skin dan energy, Bag untuk informasi artefak, Back to Top bersyarat, dan sistem artefak berdasarkan rarity dan depth.
* Dynamics: Strategi pemain dalam memilih jalur penggalian, mengatur energy, menentukan kapan kembali ke permukaan, membeli energy di Shop, dan mengejar artefak sesuai depth yang disarankan.
* Aesthetics: Perasaan santai di area permukaan dengan awan bergerak looping, sensasi eksplorasi saat menggali tanah, kepuasan dari efek percikan tanah, serta rasa pencapaian saat menemukan artefak langka.

BAB III: PERANCANGAN SISTEM

3.1 Mekanik dan Core Loop

Core loop permainan "Sworn Treasure" berjalan sebagai berikut:

1. Pemain memulai permainan di area permukaan dengan Paw Durability penuh, yaitu 50.
2. Pemain menggali tanah ke bawah, kiri, atau kanan. Setiap penggalian mengurangi Paw Durability.
3. Pemain mengumpulkan item dari tanah, seperti gold, bone, energy, dan artefak.
4. Gold dan bone memberikan point, sedangkan energy memulihkan Paw Durability.
5. Artefak muncul berdasarkan chance dan rentang depth tertentu.
6. Jika pemain menemukan artefak, sistem menampilkan alert dengan nama artefak berwarna sesuai rarity dan memberikan reward point.
7. Pemain dapat membuka Bag untuk membaca informasi artefak, chance, reward, dan depth yang disarankan.
8. Pemain dapat kembali ke permukaan menggunakan Back to Top jika energy habis atau sudah melewati depth 50.
9. Back to Top tidak memulihkan energy secara otomatis.
10. Untuk memulihkan energy, pemain harus membuka Shop dan membeli +50 Energy seharga 50 Points.
11. Pemain juga dapat menggunakan Shop untuk membeli atau memilih skin karakter.
12. Setelah siap, pemain dapat menggali kembali untuk mengejar artefak yang lebih dalam.

3.2 Sistem Artefak Nusantara

Sistem artefak dibagi menjadi lima jenis. Setiap artefak memiliki rarity, chance, depth yang disarankan, dan reward point.

Common: Pecahan Tembikar Trowulan
* Depth utama: 0-40
* Chance pada depth utama: 0.3%
* Chance setelah depth 40: 0.15%
* Reward: +30 Points
* Deskripsi: Kepingan tanah liat peninggalan Majapahit yang sering ditemukan di lapisan dangkal.

Uncommon: Koin Gobog Wayang
* Sebelum depth 40: 0% chance
* Depth utama: 40-80
* Chance pada depth utama: 0.2%
* Chance setelah depth 80: 0.1%
* Reward: +40 Points
* Deskripsi: Koin kuno berlubang di tengah dengan ukiran figur mirip wayang.

Rare: Kapak Corong Perunggu
* Sebelum depth 80: 0% chance
* Depth utama: 80-120
* Chance pada depth utama: 0.1%
* Chance setelah depth 120: 0.08%
* Reward: +60 Points
* Deskripsi: Artefak prasejarah berbentuk kapak dengan bagian atas berongga.

Ultra Rare: Arca Perunggu Ganesha
* Sebelum depth 120: 0% chance
* Depth utama: 120-150
* Chance pada depth utama: 0.075%
* Chance setelah depth 150: 0.045%
* Reward: +75 Points
* Deskripsi: Patung kecil era klasik dari perunggu dengan detail ukiran rumit.

Mythical: Bokor Emas Wonoboyo
* Sebelum depth 150: 0% chance
* Depth utama: 150-300
* Chance pada depth utama: 0.03%
* Reward: +100 Points
* Deskripsi: Mangkuk emas berelief Ramayana dari penemuan harta karun besar di Jawa Tengah.

3.3 Sistem Shop dan Energy

Shop berfungsi sebagai tempat pemain menggunakan point. Pada implementasi terbaru, Shop memiliki dua fungsi utama:

* Membeli atau memilih skin karakter.
* Membeli energy dengan harga 50 Points untuk memulihkan hingga +50 Energy.

Energy tidak lagi otomatis penuh saat pemain menggunakan Back to Top. Dengan demikian, pemain perlu mengelola point dan energy secara lebih strategis. Jika energy sudah penuh, pembelian energy tidak dilakukan agar point pemain tidak terbuang.

3.4 Sistem Back to Top

Back to Top adalah tombol untuk mengembalikan karakter ke area permukaan. Sistem ini memiliki syarat agar tidak dapat di-spam. Tombol hanya dapat digunakan jika:

* Energy pemain sudah habis, atau
* Pemain sudah berada lebih dalam dari depth 50.

Ketika Back to Top digunakan, posisi karakter kembali ke permukaan dan map dapat direset, tetapi energy pemain tidak dipulihkan otomatis. Pemulihan energy dilakukan melalui Shop.

3.5 Sistem Bag

Bag adalah panel informasi artefak. Panel ini berisi:

* Nama artefak.
* Rarity dengan warna yang berbeda.
* Preview gambar artefak.
* Chance kemunculan.
* Depth yang disarankan.
* Reward point.
* Deskripsi singkat artefak.

Bag membantu pemain memahami target eksplorasi dan memberi konteks edukatif mengenai artefak asli Indonesia.

3.6 Perancangan Antarmuka (UI/UX)

Antarmuka game dirancang agar mudah dibaca dan mendukung keputusan pemain. Elemen utama UI meliputi:

* HUD point untuk menampilkan total point pemain.
* HUD energy atau Paw Durability untuk menampilkan sisa energy.
* Tombol Shop untuk membuka panel pembelian skin dan energy.
* Tombol Bag untuk membuka informasi artefak.
* Tombol Back to Top untuk kembali ke permukaan dengan syarat tertentu.
* Alert berwarna untuk memberi feedback saat menemukan artefak atau gagal membeli item.

ShopPanel diperbesar agar pembelian energy dapat masuk tanpa mengganggu item skin. BagPanel diperbesar dan ditata agar preview gambar artefak sejajar dengan teks informasi. Rarity pada BagPanel dan alert diberi warna berbeda agar pemain cepat mengenali tingkat kelangkaan artefak.

3.7 Audio dan Feedback Visual

Feedback visual dan audio digunakan untuk meningkatkan kepuasan bermain:

* SFX menggali saat karakter menghancurkan tile tanah.
* Efek partikel tanah kecil saat dog menggali.
* Alert saat menemukan artefak, membeli item, atau gagal membeli item.
* Awan di background bergerak looping dari kanan ke kiri agar area permukaan terasa hidup.

3.8 Pembagian Tugas Tim

Programmer
* Membuat grid system, tilemap generation, logic pengurangan energy, sistem point, sistem artefak, Shop, Bag, Back to Top, alert, dan efek penggalian.

Artist
* Membuat sprite karakter anjing, environment, tile tanah, item tanah, artefak Nusantara, icon HUD, dan elemen UI Shop/Bag.

Sound Designer
* Menyediakan SFX menggali, SFX tombol, SFX pembelian, SFX alert, serta BGM yang sesuai dengan suasana permainan.

BAB IV: RENCANA IMPLEMENTASI DAN PENGUJIAN

4.1 Rencana Implementasi

Permainan dikembangkan menggunakan game engine 2D dengan sistem tilemap. Semua elemen tanah, item, dan artefak disusun mengikuti grid agar interaksi penggalian stabil. Sistem generation map menempatkan tile tanah, item point, energy, dan artefak berdasarkan chance. Kedalaman maksimal permainan saat ini adalah 300 depth.

Implementasi UI dilakukan melalui Canvas dengan beberapa panel utama, yaitu HUD, ShopPanel, BagPanel, alert panel, tombol Back to Top, dan tombol Shop/Bag. ShopPanel memuat skin dan pembelian energy. BagPanel memuat data artefak dan preview gambar yang dapat diedit melalui hierarchy.

4.2 Skenario Pengujian

Pengujian difokuskan pada mekanik inti menggunakan Black Box Testing:

* Pengujian Movement dan Digging: Memastikan karakter dapat menggali ke bawah, kiri, dan kanan sesuai grid.
* Pengujian Durability: Memastikan energy berkurang saat menggali dan tidak bernilai negatif.
* Pengujian Energy Tile: Memastikan item energy dapat menambah Paw Durability sampai batas maksimal.
* Pengujian Point: Memastikan item point dan artefak menambah point sesuai nilai reward.
* Pengujian Artifact Chance: Memastikan artefak muncul sesuai depth minimum, depth range, chance, dan tile asset yang dipasang.
* Pengujian Alert Rarity: Memastikan alert menampilkan nama artefak dengan warna sesuai rarity.
* Pengujian BagPanel: Memastikan gambar, teks, rarity, chance, depth, dan reward dapat dibaca dengan jelas.
* Pengujian Shop Energy: Memastikan pembelian energy memotong 50 Points dan memulihkan energy sampai batas maksimal.
* Pengujian Shop Guard: Memastikan pembelian energy gagal jika point kurang atau energy sudah penuh.
* Pengujian Back to Top: Memastikan tombol hanya aktif secara fungsi ketika energy habis atau depth lebih dari 50, serta tidak mereset energy.
* Pengujian Cloud Loop: Memastikan awan bergerak dari kanan ke kiri secara looping tanpa cut yang terlihat.
* Pengujian Dig Particle: Memastikan efek percikan tanah muncul saat menggali dan tidak mengganggu gameplay.

BAB V: PENUTUP

5.1 Kesimpulan

Game "Sworn Treasure" merupakan game eksplorasi 2D berbasis grid yang menggabungkan mekanik penggalian, manajemen energy, sistem point, Shop, Bag, dan penemuan artefak Nusantara. Sistem Paw Durability mendorong pemain untuk berpikir strategis dalam menggali dan mengatur resource. Sistem artefak berdasarkan rarity dan depth memberi motivasi bagi pemain untuk menggali lebih dalam dan memahami informasi artefak melalui BagPanel.

Pengembangan UI/UX seperti ShopPanel, BagPanel, alert berwarna, tombol Back to Top bersyarat, serta efek visual penggalian membuat pengalaman bermain lebih jelas, informatif, dan memuaskan. Dengan konsep ini, Sworn Treasure dapat menjadi game kasual yang tidak hanya menghibur tetapi juga memperkenalkan artefak budaya Indonesia secara ringan.

5.2 Saran

Ke depannya, sistem artefak dapat diperluas menjadi sistem koleksi permanen, galeri artefak, atau achievement. Variasi tile tanah, efek visual untuk artefak Mythical, animasi karakter, audio khusus setiap rarity, serta balancing chance dan reward juga dapat dikembangkan untuk memperkuat replayability.

DAFTAR PUSTAKA

[Daftar pustaka akan disesuaikan dengan literatur yang dirujuk, berikut adalah placeholder berdasarkan literatur yang relevan dengan Game 2D, UI/UX, dan MDA Framework:]

* Alvian, M. B. R., dkk. (2025). Implementasi Logika dalam Game 2D.
* Ridwan, R. M., dkk. (2024). Perancangan Game dengan Pendekatan MDA Framework.
* Hunicke, R., LeBlanc, M., & Zubek, R. (2004). MDA: A Formal Approach to Game Design and Game Research.
