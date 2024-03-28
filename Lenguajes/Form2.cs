using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using static Lenguajes.Form2;

namespace Lenguajes
{
    public partial class Form2 : Form
    {
        private City city; 
        private Dictionary<string, Image> imagenesPorNombre;
        Random rnd = new Random(Guid.NewGuid().GetHashCode());
        private string start;
        private string end;
        int obstacle_count;
        private List<Point> path = new List<Point>(); // Almacena el camino encontrado
        private Color colorDijkstra = Color.Green;
        private Color colorAStar = Color.Magenta;
        private Color currentPathColor;


        public Form2(string start, string end, int obstacle_count)
        {
            this.start = start;
            this.end = end;
            this.obstacle_count = obstacle_count;

            InitializeComponent();
            city = new City(1100, 610); 

            city.AddPlace("UNITEC", new Point(100, 100), Properties.Resources.unitec);
            city.AddPlace("LA COLONIA", new Point(500, 300), Properties.Resources.colonia);
            city.AddPlace("AEROPUERTO", new Point(1000, 30), Properties.Resources.aeropuerto);
            city.AddPlace("MALL", new Point(1080, 550), Properties.Resources.city);
            city.AddPlace("CASA", new Point(50, 500), Properties.Resources.casa);
            city.ConnectPlacesBasedOnProximity(obstacle_count);


            this.Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

           
            Pen streetPen = new Pen(Color.Blue, 4);
            Pen streetPen2 = new Pen(Color.Red, 4);

            
            Brush brush = Brushes.Red;
            foreach (var street in city.Streets)
            {
                foreach (var segment in street.Segments)
                {
                    g.DrawLine(streetPen, segment.Start, segment.End); 

                    if (segment.hasObstacle)
                    {
                        
                        int centerX = (segment.Start.X + segment.End.X) / 2;
                        int centerY = (segment.Start.Y + segment.End.Y) / 2;

                        // Define el tamaño de la "X"
                        int size = 10; // Puedes cambiar esto para que la "X" sea más grande o más pequeña

                        // Dibuja la primera línea de la "X"
                        g.DrawLine(streetPen2, centerX - size, centerY - size, centerX + size, centerY + size);

                        // Dibuja la segunda línea de la "X"
                        g.DrawLine(streetPen2, centerX - size, centerY + size, centerX + size, centerY - size);
                    }

                }
            }

            
            foreach (var place in city.Places)
            {
                
                if (place.Image != null)
                {
                    g.DrawImage(place.Image, new Rectangle(place.Location, new Size(50, 50))); 
                }

                g.DrawString(place.Name, this.Font, Brushes.Black, new PointF(place.Location.X, place.Location.Y + 55)); 

                g.FillEllipse(Brushes.Red, place.Location.X - 5, place.Location.Y - 5, 10, 10); 
            }

            if (path.Count > 1)
            {
                Pen pathPen = new Pen(currentPathColor, 5); // Usa el color actual del camino
                for (int i = 0; i < path.Count - 1; i++)
                {
                    g.DrawLine(pathPen, path[i], path[i + 1]); // Dibuja una línea entre cada par de puntos en el camino
                }
                pathPen.Dispose(); // Libera los recursos del objeto Pen
            }

            streetPen.Dispose();
        }

        public class Place
        {
            public string Name { get; set; }
            public Point Location { get; set; }
            public Image Image { get; set; }
        }

        public class StreetSegment
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public bool hasObstacle { get; set; }

            public double Weight { get; set; } // Peso basado en la distancia y obstáculos

        }

        public class Street
        {
            public List<StreetSegment> Segments { get; set; }

            public Street()
            {
                Segments = new List<StreetSegment>();
            }

            public void AddSegment(Point Start, Point End, bool obstacle)
            {
                Segments.Add(new StreetSegment { Start = Start, End = End, hasObstacle = obstacle });
            }
        }



        public class City
        {
            public Place[,] DataGrid { get; private set; }
            public List<Place> Places { get; private set; }
            public List<Street> Streets { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public double CalculateWeight(StreetSegment segment)
            {
                double baseWeight = CalculateDistance(segment.Start, segment.End);
                return segment.hasObstacle ? baseWeight * 10 : baseWeight; // Aumenta el peso por un factor si hay un obstáculo.
            }
            // Método para inicializar los pesos de todos los segmentos.
            public void InitializeWeights()
            {
                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        segment.Weight = CalculateWeight(segment);
                    }
                }
            }

            public City(int width, int height)
            {
                Width = width;
                Height = height;
                DataGrid = new Place[width, height];
                Places = new List<Place>();
                Streets = new List<Street>();
            }

         
            public List<Point> FindPath(string startName, string endName)
            {
                // Obtener los lugares de inicio y fin
                Place startPlace = Places.FirstOrDefault(p => p.Name == startName);
                Place endPlace = Places.FirstOrDefault(p => p.Name == endName);

                if (startPlace == null || endPlace == null)
                {
                    throw new ArgumentException("Start or end place not found.");
                }

                // Verificar si el lugar de destino es alcanzable desde el lugar de inicio
                if (!IsConnected(startPlace, endPlace))
                {
                    MessageBox.Show("NO ESTAN CONECTADOS");
                }

                // Reconstruir el camino hacia el lugar de destino
                List<Point> path = new List<Point>();
                Point currentPlace = endPlace.Location;

                while (currentPlace != Point.Empty)
                {
                    path.Insert(0, currentPlace); // Insertar en la primera posición para mantener el orden correcto
                    currentPlace = GetPreviousPoint(currentPlace);
                }

                return path;
            }

            private Point GetPreviousPoint(Point point)
            {
                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        if (segment.End == point)
                        {
                            return segment.Start;
                        }
                    }
                }
                return Point.Empty; // No se encontró un punto anterior
            }

            private bool IsConnected(Place start, Place end)
            {
                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        if ((segment.Start == start.Location && segment.End == end.Location) ||
                            (segment.Start == end.Location && segment.End == start.Location))
                        {
                            return true; // Hay una conexión directa entre los dos lugares
                        }
                    }
                }
                return false; // No hay una conexión directa entre los dos lugares
            }




            public void AddSquareCurveStreet(Place start, Place end)
            {
                // Calcula puntos intermedios para los giros cuadrados
                // Elige un punto para girar desde la posición X de start y la posición Y de end, o viceversa
                Point turnPoint = new Point(start.Location.X, end.Location.Y);

                Street street = new Street();
                street.AddSegment(start.Location, turnPoint, false); // Primer segmento horizontal o vertical
                street.AddSegment(turnPoint, end.Location, false);   // Segundo segmento perpendicular al primero
                Streets.Add(street);
            }

            public void AddPlace(string name, Point location, Image image)
            {
                if (location.X < Width && location.Y < Height)
                {
                    var place = new Place { Name = name, Location = location, Image = image };
                    DataGrid[location.X, location.Y] = place;
                    Places.Add(place);
                }
            }



            public int AddMultipleSquareCurveStreet(Place start, Place end, int obstacle_count)
            {


                // Definir los límites dentro de los cuales las calles pueden ser creadas
                int xMax = 1000, yMax = 500;
                int xMin = 50, yMin = 30;

                // Lista para almacenar los puntos de giro
                List<Point> turningPoints = new List<Point>();

                // Asegurarse de que el primer punto de giro esté dentro de los límites
                int firstTurnX = Math.Max(xMin, Math.Min(start.Location.X, xMax));
                int firstTurnY = Math.Max(yMin, Math.Min((start.Location.Y + end.Location.Y) / 2, yMax));
                turningPoints.Add(new Point(firstTurnX, firstTurnY));

                // Añadir puntos intermedios adicionales, asegurándose de que están dentro de los límites
                int numberOfTurns = 2; // Número de giros adicionales
                int deltaX = (end.Location.X - start.Location.X) / (numberOfTurns + 1);
                int deltaY = (end.Location.Y - start.Location.Y) / (numberOfTurns + 1);

                for (int i = 1; i <= numberOfTurns; i++)
                {
                    Point lastPoint = turningPoints.Last();
                    // Añadir un giro en la dirección X, asegurándose de que está dentro de los límites
                    int nextTurnX = Math.Max(xMin, Math.Min(lastPoint.X + deltaX, xMax));
                    int nextTurnY = lastPoint.Y;
                    turningPoints.Add(new Point(nextTurnX, nextTurnY));

                    // Añadir un giro en la dirección Y, asegurándose de que está dentro de los límites
                    if (i != numberOfTurns) // Evitar añadir un punto adicional al final
                    {
                        nextTurnX = turningPoints.Last().X;
                        nextTurnY = Math.Max(yMin, Math.Min(lastPoint.Y + deltaY, yMax));
                        turningPoints.Add(new Point(nextTurnX, nextTurnY));
                    }
                }

                // Asegurarse de que el último punto de giro esté dentro de los límites
                int lastTurnX = Math.Max(xMin, Math.Min(end.Location.X, xMax));
                int lastTurnY = Math.Max(yMin, Math.Min(turningPoints.Last().Y, yMax));
                turningPoints.Add(new Point(lastTurnX, lastTurnY));

                // Crear la calle con múltiples segmentos
                Street street = new Street();

                // Añadir segmentos de la calle y agregar aleatoriamente algun obstaculo
                Point currentStart = start.Location;
                bool noObstacle = true;
                double obstacle_chance = 0.20;
                Random rand = new Random(Guid.NewGuid().GetHashCode());

                foreach (Point turnPoint in turningPoints)
                {
                    if (obstacle_count > 0 && noObstacle)
                    {
                        double var = rand.NextDouble();

                        if (var <= obstacle_chance)
                        {
                            street.AddSegment(currentStart, turnPoint, true);
                            currentStart = turnPoint;
                            noObstacle = false;
                            obstacle_count--;
                        }

                    }
                    else
                    {
                        street.AddSegment(currentStart, turnPoint, false);
                        currentStart = turnPoint;
                    }


                }

                // Añadir el último segmento hacia el lugar de destino
                street.AddSegment(turningPoints.Last(), end.Location, false);

                Streets.Add(street);

                return obstacle_count;
            }


            public void ConnectPlacesBasedOnProximity(int obstacle_count)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                int maxConnections = 8; 

                foreach (var place in Places)
                {
                    int numberOfConnections = rnd.Next(1, maxConnections + 1);

                    var possibleConnections = Places.Except(new[] { place })
                                                    .OrderBy(x => rnd.Next()) 
                                                    .Take(numberOfConnections)
                                                    .ToList();

                    foreach (var target in possibleConnections)
                    {
                        if (!IsConnected(place, target))
                        {
                            obstacle_count = AddMultipleSquareCurveStreet(place, target, obstacle_count);
                        }
                        else
                        {

                        }
                    }
                }
            }

            public bool StreetHasObstacle(Point start, Point end)
            {
                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        if ((segment.Start.Equals(start) && segment.End.Equals(end)) ||
                            (segment.Start.Equals(end) && segment.End.Equals(start)))
                        {
                            return segment.hasObstacle;
                        }
                    }
                }
                return false; // Si no se encuentra un segmento, asume que no hay obstáculo
            }



            private double CalculateDistance(Point point1, Point point2)
            {
                return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
            }


            //A POINTER =========================
            public List<Point> AStar(Point start, Point end)
            {
                HashSet<Point> closedSet = new HashSet<Point>();
                HashSet<Point> openSet = new HashSet<Point>(); 
                Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
                Dictionary<Point, double> gScore = new Dictionary<Point, double>(); // Costo real desde el inicio al nodo
                Dictionary<Point, double> fScore = new Dictionary<Point, double>(); // Costo estimado desde el inicio al nodo más el costo estimado desde el nodo al objetivo

                foreach (var place in Places)
                {
                    gScore[place.Location] = double.MaxValue;
                    fScore[place.Location] = double.MaxValue; 
                }


               
                openSet.Add(start);

                while (openSet.Count > 0)
                {
                   
                    Point current = GetLowestFScoreNode(openSet, fScore);

                    // Si el nodo actual es el nodo objetivo, reconstruir y devolver el camino
                    if (current == end)
                    {
                        return ReconstructPath(cameFrom, current);
                    }

                  
                    openSet.Remove(current);
                    closedSet.Add(current);

                    // Explorar los vecinos del nodo actual
                    foreach (var neighbor in GetNeighbors(current))
                    {
                        if (closedSet.Contains(neighbor))
                        {
                            continue; // Saltar este vecino, ya ha sido evaluado
                        }

                        // Calcular el costo real desde el inicio hasta este vecino
                        double currentGScore;
                        if (!gScore.TryGetValue(current, out currentGScore))
                        {
                            currentGScore = double.MaxValue;
                        }
                        double tentativeGScore = currentGScore + CalculateDistance(current, neighbor);

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor); // Añadir el vecino al conjunto abierto si no está allí
                        }
                        else if (tentativeGScore >= gScore[neighbor])
                        {
                            continue; // Este no es un mejor camino
                        }

                        // Este camino es el mejor hasta ahora. Registrarlo.
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + CalculateDistance(neighbor, end);
                    }
                }

                // No se encontró un camino
                return null;
            }
            public Place GetPlace(string name)
            {
                return Places.FirstOrDefault(p => p.Name == name);
            }

            private Point GetLowestFScoreNode(HashSet<Point> openSet, Dictionary<Point, double> fScore)
            {
                Point lowestNode = default(Point);
                double lowestScore = double.MaxValue;

                foreach (var node in openSet)
                {
                    if (fScore.ContainsKey(node) && fScore[node] < lowestScore)
                    {
                        lowestScore = fScore[node];
                        lowestNode = node;
                    }
                }

                return lowestNode;
            }

            private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
            {
                List<Point> path = new List<Point>();
                path.Add(current); 

                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }

                return path;
            }


            public List<Point> Dijkstra(Point start, Point end)
            {
                var openSet = new HashSet<Point>(); 
                var cameFrom = new Dictionary<Point, Point>(); 
                var gScore = new Dictionary<Point, double>(); 

                // Inicializar todos los nodos con un costo infinito, excepto el inicio
                foreach (var place in Places)
                {
                    gScore[place.Location] = double.MaxValue;
                }
                gScore[start] = 0;

                openSet.Add(start);

                while (openSet.Count > 0)
                {
                    // Seleccionar el nodo con el menor gScore
                    Point current = openSet.OrderBy(n => gScore.ContainsKey(n) ? gScore[n] : double.MaxValue).First();

                    if (current == end)
                    {
                        return ReconstructPath(cameFrom, current);
                    }

                    openSet.Remove(current);

                    foreach (var neighbor in GetNeighbors(current))
                    {
                        // Verificar si hay un obstáculo en el segmento de la calle
                        if (StreetHasObstacle(current, neighbor))
                        {
                            // Si hay un obstáculo, no considerar este vecino
                            continue;
                        }

                        // Initialize the neighbor in gScore if it hasn't been added yet
                        if (!gScore.ContainsKey(neighbor))
                        {
                            gScore[neighbor] = double.MaxValue;
                        }

                        // Calculate the tentative gScore for the neighbor
                        double tentativeGScore = gScore[current] + CalculateDistance(current, neighbor);

                        if (tentativeGScore < gScore[neighbor])
                        {
                            
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeGScore;

                            if (!openSet.Contains(neighbor))
                            {
                                openSet.Add(neighbor);
                            }
                        }
                    }
                }

                // Si llegamos aquí, no se encontró un camino
                return null;
            }
            public void ResetAndAddObstacles(int obstacleCount)
            {
                // Eliminar todos los obstáculos existentes
                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        segment.hasObstacle = false; // Elimina el obstáculo existente
                    }
                }

                // Añadir nuevos obstáculos de forma aleatoria
                Random random = new Random();
                List<StreetSegment> allSegments = Streets.SelectMany(s => s.Segments).ToList();

                for (int i = 0; i < obstacleCount; i++)
                {
                    if (allSegments.Count == 0)
                    {
                        break; 
                    }

                    int index = random.Next(allSegments.Count);
                    StreetSegment randomSegment = allSegments[index];

                   
                    if (!randomSegment.hasObstacle)
                    {
                        randomSegment.hasObstacle = true; // Añade el obstáculo
                    }
                    else
                    {
                        i--; // Decrementa el contador y prueba de nuevo
                    }

                   
                    allSegments.RemoveAt(index);
                }
            }




            private List<Point> GetNeighbors(Point current)
            {
                List<Point> neighbors = new List<Point>();

                foreach (var street in Streets)
                {
                    foreach (var segment in street.Segments)
                    {
                        if (segment.Start == current)
                        {
                            neighbors.Add(segment.End);
                        }
                        else if (segment.End == current)
                        {
                            neighbors.Add(segment.Start);
                        }
                    }
                }

                return neighbors;
            }
        }


        public class AStar
        {
            private readonly List<Point> _path;
            private readonly HashSet<Point> _closedSet;
            private readonly HashSet<Point> _openSet;
            private readonly Dictionary<Point, Point> _cameFrom;
            private readonly Dictionary<Point, double> _gScore;
            private readonly Dictionary<Point, double> _fScore;

            private readonly City _city;

            public AStar(City city)
            {
                _city = city;
                _path = new List<Point>();
                _closedSet = new HashSet<Point>();
                _openSet = new HashSet<Point>();
                _cameFrom = new Dictionary<Point, Point>();
                _gScore = new Dictionary<Point, double>();
                _fScore = new Dictionary<Point, double>();
            }

            public List<Point> FindPath(Point start, Point goal)
            {
                _openSet.Add(start);
                _gScore[start] = 0;
                _fScore[start] = HeuristicCostEstimate(start, goal);

                while (_openSet.Any())
                {
                    var current = GetLowestFScoreNode();

                    if (current == goal)
                    {
                        return ReconstructPath(current);
                    }

                    _openSet.Remove(current);
                    _closedSet.Add(current);

                    foreach (var neighbor in GetNeighbors(current))
                    {
                        if (_closedSet.Contains(neighbor))
                        {
                            continue;
                        }

                        var tentativeGScore = _gScore[current] + DistanceBetween(current, neighbor);

                        if (!_openSet.Contains(neighbor) || tentativeGScore < _gScore[neighbor])
                        {
                            _cameFrom[neighbor] = current;
                            _gScore[neighbor] = tentativeGScore;
                            _fScore[neighbor] = _gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);

                            if (!_openSet.Contains(neighbor))
                            {
                                _openSet.Add(neighbor);
                            }
                        }
                    }
                }

                // Path not found
                return null;
            }

            private Point GetLowestFScoreNode()
            {
                return _openSet.OrderBy(node => _fScore.ContainsKey(node) ? _fScore[node] : double.MaxValue).First();
            }

            private double HeuristicCostEstimate(Point start, Point goal)
            {
                
                return Math.Sqrt(Math.Pow(goal.X - start.X, 2) + Math.Pow(goal.Y - start.Y, 2));
            }
            private List<Point> GetNeighbors(Point current)
            {
                List<Point> neighbors = new List<Point>();

                foreach (var street in _city.Streets) 
                {
                    foreach (var segment in street.Segments)
                    {
                        if (segment.hasObstacle) 
                            continue;

                        if (segment.Start == current)
                        {
                            neighbors.Add(segment.End);
                        }
                        else if (segment.End == current)
                        {
                            neighbors.Add(segment.Start);
                        }
                    }
                }

                return neighbors;
            }



            private List<Point> ReconstructPath(Point current)
            {
                _path.Clear();
                _path.Add(current);

                while (_cameFrom.ContainsKey(current))
                {
                    current = _cameFrom[current];
                    _path.Insert(0, current);
                }

                return _path;
            }

            private double DistanceBetween(Point start, Point end)
            {
              
                return Math.Sqrt(Math.Pow(end.X - start.X, 2) + Math.Pow(end.Y - start.Y, 2));
            }
        }


        private void InitializeComponent()
        {
            button3 = new Button();
            button6 = new Button();
            cb_lugarA2 = new ComboBox();
            cb_lugarB2 = new ComboBox();
            numericUpDown12 = new NumericUpDown();
            btn_viajar = new Button();
            button7 = new Button();
            ((System.ComponentModel.ISupportInitialize)numericUpDown12).BeginInit();
            SuspendLayout();
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Location = new Point(12, 9);
            button3.Name = "button3";
            button3.Size = new Size(168, 47);
            button3.TabIndex = 0;
            button3.Text = "APOINTER";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button6.Location = new Point(186, 9);
            button6.Name = "button6";
            button6.Size = new Size(174, 47);
            button6.TabIndex = 1;
            button6.Text = "DIJKSTRA";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // cb_lugarA2
            // 
            cb_lugarA2.FormattingEnabled = true;
            cb_lugarA2.Items.AddRange(new object[] { "UNITEC", "LA COLONIA", "CASA", "MALL", "AEROPUERTO" });
            cb_lugarA2.Location = new Point(268, 573);
            cb_lugarA2.Name = "cb_lugarA2";
            cb_lugarA2.Size = new Size(179, 23);
            cb_lugarA2.TabIndex = 3;
            // 
            // cb_lugarB2
            // 
            cb_lugarB2.FormattingEnabled = true;
            cb_lugarB2.Items.AddRange(new object[] { "UNITEC", "LA COLONIA", "CASA", "MALL", "AEROPUERTO" });
            cb_lugarB2.Location = new Point(462, 573);
            cb_lugarB2.Name = "cb_lugarB2";
            cb_lugarB2.Size = new Size(179, 23);
            cb_lugarB2.TabIndex = 5;
            // 
            // numericUpDown12
            // 
            numericUpDown12.Location = new Point(659, 573);
            numericUpDown12.Margin = new Padding(3, 2, 3, 2);
            numericUpDown12.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numericUpDown12.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown12.Name = "numericUpDown12";
            numericUpDown12.Size = new Size(178, 23);
            numericUpDown12.TabIndex = 9;
            numericUpDown12.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btn_viajar
            // 
            btn_viajar.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_viajar.Location = new Point(872, 562);
            btn_viajar.Name = "btn_viajar";
            btn_viajar.Size = new Size(284, 38);
            btn_viajar.TabIndex = 10;
            btn_viajar.Text = "Viajar";
            btn_viajar.UseVisualStyleBackColor = true;
            btn_viajar.Click += btn_viajar_Click;
            // 
            // button7
            // 
            button7.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button7.Location = new Point(1183, 11);
            button7.Name = "button7";
            button7.Size = new Size(88, 38);
            button7.TabIndex = 11;
            button7.Text = "VS";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // Form2
            // 
            BackgroundImage = Properties.Resources.detailed_map_of_tegucigalpa1;
            ClientSize = new Size(1283, 612);
            Controls.Add(button7);
            Controls.Add(btn_viajar);
            Controls.Add(numericUpDown12);
            Controls.Add(cb_lugarB2);
            Controls.Add(cb_lugarA2);
            Controls.Add(button6);
            Controls.Add(button3);
            Name = "Form2";
            FormClosed += Form2_FormClosed;
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDown12).EndInit();
            ResumeLayout(false);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentPathColor = colorAStar; // Establecer el color para A*

            // Ejecutar el algoritmo A*
            AStar astar = new AStar(city);
            try
            {
                path = astar.FindPath(city.GetPlace(start).Location, city.GetPlace(end).Location);
                // Utiliza el resultado shortestPath como lo necesites
                StringBuilder pathDescription = new StringBuilder();
                pathDescription.AppendLine($"Camino de {start} a {end}:");

                foreach (Point point in path)
                {
                    pathDescription.AppendLine($"({point.X} -> {point.Y})");
                }

                // Muestra el camino en un MessageBox
                MessageBox.Show(pathDescription.ToString(), "Camino Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
               
            }


            this.Invalidate();

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                currentPathColor = colorDijkstra; // Establecer el color para Dijkstra

                Place startPlace = city.GetPlace(start);
                Place endPlace = city.GetPlace(end);

                if (startPlace != null && endPlace != null)
                {
                    path = city.Dijkstra(startPlace.Location, endPlace.Location);

                    // Construye el string que representa el camino
                    StringBuilder pathDescription = new StringBuilder();
                    pathDescription.AppendLine($"Camino de {start} a {end}:");

                    foreach (Point point in path)
                    {
                        pathDescription.AppendLine($"({point.X}, {point.Y})");
                    }

                    // Muestra el camino en un MessageBox
                    MessageBox.Show(pathDescription.ToString(), "Camino Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No se encontraron uno o ambos lugares.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular el camino: {ex.Message}");
            }

            this.Invalidate();
        }

        private void btn_viajar_Click(object sender, EventArgs e)
        {
            string start = (string)cb_lugarA2.SelectedItem;
            string end = (string)cb_lugarB2.SelectedItem;
            int obstacles = (int)numericUpDown12.Value;
            this.start = start;
            this.end = end;
            this.obstacle_count = obstacles; // Corrección aquí

            city.ResetAndAddObstacles(obstacles); // Reinicia y añade nuevos obstáculos

            this.Invalidate(); // Fuerza el redibujo del formulario para mostrar los cambios

        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {

        }

        // Ejemplo de estructura para almacenar los resultados de los algoritmos y sus métricas
        public struct AlgorithmResult
        {
            public double PathLength;
            public long ExecutionTime; // Milisegundos
            public int VisitedNodes;
        }

        // Método hipotético para ejecutar A* y obtener métricas
        private AlgorithmResult RunAStarAndGetMetrics()
        {
            // Aquí iría la lógica para ejecutar A* y medir las métricas de interés
            // Por ejemplo: longitud del camino, tiempo de ejecución, nodos visitados, etc.
            return new AlgorithmResult { PathLength = 100, ExecutionTime = 50, VisitedNodes = 200 };
        }

        // Método hipotético para ejecutar Dijkstra y obtener métricas
        private AlgorithmResult RunDijkstraAndGetMetrics()
        {
            // Aquí iría la lógica para ejecutar Dijkstra y medir las métricas de interés
            return new AlgorithmResult { PathLength = 120, ExecutionTime = 40, VisitedNodes = 180 };
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();

            // Ejecutar A*
            stopwatch.Start();
            var aStarPath = RunAStar();
            stopwatch.Stop();
            long aStarTicks = stopwatch.ElapsedTicks;
            long aStarTimeNs = (aStarTicks * 1000000000) / Stopwatch.Frequency;
            int aStarPathLength = aStarPath.Count;

            // Reiniciar el cronómetro
            stopwatch.Reset();

            // Ejecutar Dijkstra
            stopwatch.Start();
            var dijkstraPath = RunDijkstra();
            stopwatch.Stop();
            long dijkstraTicks = stopwatch.ElapsedTicks;
            long dijkstraTimeNs = (dijkstraTicks * 1000000000) / Stopwatch.Frequency;
            int dijkstraPathLength = dijkstraPath.Count;

            // Construir el mensaje de comparación
            StringBuilder comparisonMessage = new StringBuilder();
            comparisonMessage.AppendLine("Comparación entre A* y Dijkstra:");
            comparisonMessage.AppendLine();
            comparisonMessage.AppendLine($"- Longitud del camino A*: {aStarPathLength}");
            comparisonMessage.AppendLine($"- Longitud del camino Dijkstra: {dijkstraPathLength}");
            comparisonMessage.AppendLine($"- Tiempo de ejecución A*: {aStarTimeNs}ns");
            comparisonMessage.AppendLine($"- Tiempo de ejecución Dijkstra: {dijkstraTimeNs}ns");

            // Mostrar el mensaje en un MessageBox
            MessageBox.Show(comparisonMessage.ToString(), "Comparación A* vs Dijkstra", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        private List<Point> RunAStar()
        {
            AStar astar = new AStar(city);
            var path = astar.FindPath(city.GetPlace(start).Location, city.GetPlace(end).Location);
            return path;
        }

        private List<Point> RunDijkstra()
        {
            Place startPlace = city.GetPlace(start);
            Place endPlace = city.GetPlace(end);
            var path = city.Dijkstra(startPlace.Location, endPlace.Location);
            return path;
        }

    }
}
