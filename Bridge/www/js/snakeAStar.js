
Bridge.Console.log = function(message) { console.log(message); };
Bridge.Console.error = function(message) { console.error(message); };
Bridge.Console.debug = function(message) { console.debug(message); };


/**
 * @version 1.0.0.0
 * @copyright Copyright ©  2017
 * @compiler Bridge.NET 15.7.0
 */
Bridge.assembly("SnakeAStar", function ($asm, globals) {
    "use strict";

    Bridge.define("SnakeAStar.ASTarSolver", {
        statics: {
            cachedPoints: null,
            neighborCache: null,
            config: {
                init: function () {
                    this.cachedPoints = new (System.Collections.Generic.List$1(SnakeAStar.Facing))();
                    this.neighborCache = System.Array.init(3, 0, SnakeAStar.Facing);
                }
            },
            getInput: function (board) {
                var $t;
                if (SnakeAStar.ASTarSolver.cachedPoints.getCount() > 0) {
                    var point = SnakeAStar.ASTarSolver.cachedPoints.getItem(0);
                    SnakeAStar.ASTarSolver.cachedPoints.removeAt(0);
                    return point;
                }
                var startTicks = window.performance.now();
                var start = board.snake.getHead();
                var goal = board.dot;

                var fakeBoard = new SnakeAStar.Board.ctor(board);

                var closedSet = new (System.Collections.Generic.List$1(System.Int32))();
                var openSet = function (_o1) {
                        _o1.add(start.hashCode);
                        return _o1;
                    }(new (System.Collections.Generic.List$1(System.Int32))());
                var cameFrom = new (System.Collections.Generic.Dictionary$2(System.Int32,SnakeAStar.Point))();

                var gScore = new (System.Collections.Generic.Dictionary$2(System.Int32,System.Double))();
                gScore.set(start.hashCode, 0);

                var fScore = new (System.Collections.Generic.List$1(Object))();
                fScore.add({ item1: new SnakeAStar.Snake.ctor(board.snake), item2: SnakeAStar.ASTarSolver.distance(start, goal) });

                while (openSet.getCount() > 0) {

                    var lowest = System.Double.max;
                    var itemIndex = -1;
                    var currentSnake = null;
                    for (var index = 0; index < fScore.getCount(); index = (index + 1) | 0) {
                        var tuple = fScore.getItem(index);
                        if (tuple.item2 <= lowest) {
                            currentSnake = tuple.item1;
                            lowest = tuple.item2;
                            itemIndex = index;
                        }
                    }

                    var currentHead = currentSnake.getHead();
                    if (currentHead.hashCodeNoFacing === goal.hashCodeNoFacing) {
                        SnakeAStar.ASTarSolver.cachedPoints = SnakeAStar.ASTarSolver.reconstruct(cameFrom, currentHead);

                        var endTicks = window.performance.now();
                        Bridge.Console.log(System.String.format("{0} ticks with {1} moves {2} length", endTicks - startTicks, SnakeAStar.ASTarSolver.cachedPoints.getCount(), currentSnake.points.getCount()));

                        //                    return cachedPoints.First();
                        return SnakeAStar.ASTarSolver.getInput(board);
                    }
                    var currentPointHashCode = currentHead.hashCode;

                    openSet.remove(currentPointHashCode);
                    closedSet.add(currentPointHashCode);
                    var newPoint = false;
                    $t = Bridge.getEnumerator(SnakeAStar.ASTarSolver.neighbors(currentHead));
                    while ($t.moveNext()) {
                        var neighbor = $t.getCurrent();
                        var newSnake = new SnakeAStar.Snake.ctor(currentSnake);
                        newSnake.setFacing(neighbor);

                        fakeBoard.snake = newSnake;

                        if (fakeBoard.tick(false)) {
                            var newHead = newSnake.getHead();

                            var neighborHashCode = newHead.hashCode;
                            if (closedSet.contains(neighborHashCode)) {
                                continue;
                            }
                            var tentative_gScore = gScore.get(currentPointHashCode) + SnakeAStar.ASTarSolver.distance(currentHead, newHead);

                            if (!openSet.contains(neighborHashCode)) {
                                openSet.add(neighborHashCode);
                            } else if (tentative_gScore >= gScore.get(neighborHashCode)) {
                                continue;
                            }

                            cameFrom.set(neighborHashCode, currentHead);
                            gScore.set(neighborHashCode, tentative_gScore);

                            fScore.add({ item1: newSnake, item2: SnakeAStar.ASTarSolver.distance(newHead, goal) });
                            newPoint = true;
                        }
                    }
                    if (!newPoint) {
                        fScore.removeAt(itemIndex);
                    }

                }

                return SnakeAStar.Facing.None;
            },
            reconstruct: function (cameFrom, current) {
                var points = new (System.Collections.Generic.List$1(SnakeAStar.Facing))();
                points.add(current.facing);

                while (cameFrom.containsKey(current.hashCode)) {
                    current = cameFrom.get(current.hashCode);
                    points.add(current.facing);
                }
                points.reverse();
                points.removeAt(0);
                return points;
            },
            neighbors: function (current) {
                switch (current.facing) {
                    case SnakeAStar.Facing.Up: 
                        SnakeAStar.ASTarSolver.neighborCache[0] = SnakeAStar.Facing.Up;
                        SnakeAStar.ASTarSolver.neighborCache[1] = SnakeAStar.Facing.Left;
                        SnakeAStar.ASTarSolver.neighborCache[2] = SnakeAStar.Facing.Right;
                        break;
                    case SnakeAStar.Facing.Down: 
                        SnakeAStar.ASTarSolver.neighborCache[0] = SnakeAStar.Facing.Down;
                        SnakeAStar.ASTarSolver.neighborCache[1] = SnakeAStar.Facing.Left;
                        SnakeAStar.ASTarSolver.neighborCache[2] = SnakeAStar.Facing.Right;
                        break;
                    case SnakeAStar.Facing.Left: 
                        SnakeAStar.ASTarSolver.neighborCache[0] = SnakeAStar.Facing.Left;
                        SnakeAStar.ASTarSolver.neighborCache[1] = SnakeAStar.Facing.Up;
                        SnakeAStar.ASTarSolver.neighborCache[2] = SnakeAStar.Facing.Down;
                        break;
                    case SnakeAStar.Facing.Right: 
                        SnakeAStar.ASTarSolver.neighborCache[0] = SnakeAStar.Facing.Right;
                        SnakeAStar.ASTarSolver.neighborCache[1] = SnakeAStar.Facing.Up;
                        SnakeAStar.ASTarSolver.neighborCache[2] = SnakeAStar.Facing.Down;
                        break;
                    default: 
                        throw new System.ArgumentOutOfRangeException();
                }
                return SnakeAStar.ASTarSolver.neighborCache;
            },
            distance: function (start, goal) {

                var x1 = ((((((goal.x - start.x) | 0)) + SnakeAStar.Program.Width) | 0)) % SnakeAStar.Program.Width;
                var y1 = ((((((goal.y - start.y) | 0)) + SnakeAStar.Program.Height) | 0)) % SnakeAStar.Program.Height;
                var x2 = ((((((start.x - goal.x) | 0)) + SnakeAStar.Program.Width) | 0)) % SnakeAStar.Program.Width;
                var y2 = ((((((start.y - goal.y) | 0)) + SnakeAStar.Program.Height) | 0)) % SnakeAStar.Program.Height;


                var result = Math.sqrt(((Math.min(((x1 * x1) | 0), ((x2 * x2) | 0)) + Math.min(((y1 * y1) | 0), ((y2 * y2) | 0))) | 0));
                /* 
                        var x1 = goal.X - start.X;
                        var y1 = goal.Y - start.Y;

                        var result = Math.Sqrt(x1 * x1 + y1 * y1);
            */

                return result;
            }
        }
    });

    Bridge.define("SnakeAStar.Board", {
        statics: {
            r: null,
            config: {
                init: function () {
                    this.r = new System.Random.$ctor1(156549);
                }
            },
            start: function (width, height, startX, startY, facing) {
                var board = new SnakeAStar.Board.$ctor1(width, height);
                board.snake = new SnakeAStar.Snake.$ctor1(startX, startY, facing);
                board.newDot();
                return board;
            }
        },
        width: 0,
        height: 0,
        snake: null,
        dot: null,
        $ctor1: function (width, height) {
            this.$initialize();
            this.width = width;
            this.height = height;
        },
        ctor: function (original) {
            this.$initialize();
            this.width = original.width;
            this.height = original.height;
            this.dot = original.dot;
            this.snake = new SnakeAStar.Snake.ctor(original.snake);
        },
        tick: function (real) {
            if (real === void 0) { real = true; }
            var movePoint;
            var snakeHead = this.snake.getHead();

            switch (snakeHead.facing) {
                case SnakeAStar.Facing.Up: 
                    movePoint = SnakeAStar.Point.getPoint(snakeHead.x, ((snakeHead.y - 1) | 0), snakeHead.facing);
                    break;
                case SnakeAStar.Facing.Down: 
                    movePoint = SnakeAStar.Point.getPoint(snakeHead.x, ((snakeHead.y + 1) | 0), snakeHead.facing);
                    break;
                case SnakeAStar.Facing.Left: 
                    movePoint = SnakeAStar.Point.getPoint(((snakeHead.x - 1) | 0), snakeHead.y, snakeHead.facing);
                    break;
                case SnakeAStar.Facing.Right: 
                    movePoint = SnakeAStar.Point.getPoint(((snakeHead.x + 1) | 0), snakeHead.y, snakeHead.facing);
                    break;
                default: 
                    throw new System.ArgumentOutOfRangeException();
            }

            if (this.snake.containsPointWithOffset(movePoint.hashCodeNoFacing, 1)) {
                return false;
            }

            if (movePoint.hashCodeNoFacing === this.dot.hashCodeNoFacing) {
                this.snake.insertPoint(movePoint);
                if (real) {
                    this.newDot();
                }
            } else {
                this.snake.insertAndMove(movePoint);
            }
            return true;
        },
        newDot: function () {
            while (true) {
                this.dot = SnakeAStar.Point.getPoint(SnakeAStar.Board.r.next$2(0, this.width), SnakeAStar.Board.r.next$2(0, this.height), SnakeAStar.Facing.None);
                if (!this.snake.containsPointWithOffset(this.dot.hashCodeNoFacing, 0)) {
                    break;
                }
            }
        }
    });

    Bridge.define("SnakeAStar.Facing", {
        $kind: "enum",
        statics: {
            Up: 1,
            Down: 2,
            Left: 3,
            Right: 4,
            None: 5
        }
    });

    Bridge.define("SnakeAStar.Point", {
        statics: {
            ctor: function () {
                for (var x = 0; x < SnakeAStar.Program.Width; x = (x + 1) | 0) {
                    for (var y = 0; y < SnakeAStar.Program.Height; y = (y + 1) | 0) {
                        SnakeAStar.Point.facingPoints.add(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + 1) | 0), new SnakeAStar.Point(x, y, SnakeAStar.Facing.Up));
                        SnakeAStar.Point.facingPoints.add(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + 2) | 0), new SnakeAStar.Point(x, y, SnakeAStar.Facing.Down));
                        SnakeAStar.Point.facingPoints.add(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + 3) | 0), new SnakeAStar.Point(x, y, SnakeAStar.Facing.Left));
                        SnakeAStar.Point.facingPoints.add(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + 4) | 0), new SnakeAStar.Point(x, y, SnakeAStar.Facing.Right));
                        SnakeAStar.Point.facingPoints.add(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + 5) | 0), new SnakeAStar.Point(x, y, SnakeAStar.Facing.None));
                    }
                }
            },
            facingPoints: null,
            config: {
                init: function () {
                    this.facingPoints = new (System.Collections.Generic.Dictionary$2(System.Int32,SnakeAStar.Point))();
                }
            },
            getPoint: function (x, y, facing) {
                if (x < 0) {
                    x = (x + SnakeAStar.Program.Width) | 0;
                } else if (x >= SnakeAStar.Program.Width) {
                    x = (x - SnakeAStar.Program.Width) | 0;
                }

                if (y < 0) {
                    y = (y + SnakeAStar.Program.Height) | 0;
                } else if (y >= SnakeAStar.Program.Height) {
                    y = (y - SnakeAStar.Program.Height) | 0;
                }

                return SnakeAStar.Point.facingPoints.get(((((((x * 100000) | 0) + ((y * 10) | 0)) | 0) + facing) | 0));
            }
        },
        hashCode: 0,
        hashCodeNoFacing: 0,
        x: 0,
        y: 0,
        facing: 0,
        ctor: function (x, y, facing) {
            this.$initialize();
            this.x = x;
            this.y = y;
            this.facing = facing;
            this.generateHashCodes();
        },
        toString: function () {
            return System.String.format("{0}: {1} {2} {3}", "Facing", this.facing, this.x, this.y);
        },
        generateHashCodes: function () {
            this.hashCode = (((((this.x * 100000) | 0) + ((this.y * 10) | 0)) | 0) + this.facing) | 0;
            this.hashCodeNoFacing = (((this.x * 100000) | 0) + this.y) | 0;
        }
    });

    Bridge.define("SnakeAStar.Program", {
        statics: {
            Width: 100,
            Height: 100,
            BlockSize: 1,
            context: null,
            getInput: function (board) {
                return SnakeAStar.ASTarSolver.getInput(board);
            },
            draw: function (board) {
                var snakeHead = board.snake.getHead();
                //            var tails = board.Snake.Tails;
                for (var y = 0; y < board.height; y = (y + 1) | 0) {
                    for (var x = 0; x < board.width; x = (x + 1) | 0) {
                        if (board.dot.x === x && board.dot.y === y) {
                            SnakeAStar.ScreenManager.setPosition(SnakeAStar.Program.context, x, y, "red");

                        } else if (board.snake.containsPoint(x, y)) {
                            if (snakeHead.x === x && snakeHead.y === y) {
                                SnakeAStar.ScreenManager.setPosition(SnakeAStar.Program.context, x, y, "green");
                            } else {
                                SnakeAStar.ScreenManager.setPosition(SnakeAStar.Program.context, x, y, "blue");

                            }
                        } else {
                            SnakeAStar.ScreenManager.setPosition(SnakeAStar.Program.context, x, y, "transparent");
                        }

                    }
                }
            }
        },
        $main: function (args) {
            //                Console.Clear();


            var canvas = Bridge.cast(document.createElement("canvas"), HTMLCanvasElement);
            canvas.width = 100;
            canvas.height = 100;
            SnakeAStar.Program.context = canvas.getContext("2d");
            SnakeAStar.Program.context.mozImageSmoothingEnabled = false;
            SnakeAStar.Program.context.msImageSmoothingEnabled = false;
            SnakeAStar.Program.context.imageSmoothingEnabled = false;
            document.body.appendChild(canvas);

            var ticks = 0;
            var board = SnakeAStar.Board.start(SnakeAStar.Program.Width, SnakeAStar.Program.Height, 3, 3, SnakeAStar.Facing.Up);
            SnakeAStar.Program.draw(board);

            var interval = 0;
            interval = window.setInterval(function () {
                var facing = SnakeAStar.Program.getInput(board);
                if (facing === SnakeAStar.Facing.None) {
                    SnakeAStar.Program.draw(board);

                    /*   
                     Window.Alert($"Dead, no moves! {board.Snake.Points.Count} Length in {ticks} ticks.");
                     Window.ClearInterval(interval);
                      return;*/
                    board = SnakeAStar.Board.start(SnakeAStar.Program.Width, SnakeAStar.Program.Height, 3, 3, SnakeAStar.Facing.Up);
                    return;
                }
                board.snake.setFacing(facing);
                if (!board.tick()) {
                    SnakeAStar.Program.draw(board);
                    window.alert(System.String.format("Dead collided! {0} Length in {1} ticks.", board.snake.points.getCount(), ticks));
                    window.clearInterval(interval);
                    return;
                }
                SnakeAStar.Program.draw(board);
                ticks = (ticks + 1) | 0;
            }, 0);
        }
    });

    Bridge.define("SnakeAStar.ScreenManager", {
        statics: {
            ctor: function () {
                for (var x = 0; x < SnakeAStar.Program.Width; x = (x + 1) | 0) {
                    for (var y = 0; y < SnakeAStar.Program.Height; y = (y + 1) | 0) {
                        SnakeAStar.ScreenManager.console[((((x * SnakeAStar.Program.Width) | 0) + y) | 0)] = "transparent";
                    }
                }
            },
            console: null,
            currentFillStyle: null,
            config: {
                init: function () {
                    this.console = System.Array.init(10000, null, String);
                }
            },
            setPosition: function (context, x, y, color) {
                if (!Bridge.referenceEquals(SnakeAStar.ScreenManager.console[((((x * SnakeAStar.Program.Width) | 0) + y) | 0)], color)) {
                    SnakeAStar.ScreenManager.console[((((x * SnakeAStar.Program.Width) | 0) + y) | 0)] = color;

                    if (Bridge.referenceEquals(color, "transparent")) {
                        context.clearRect(((x * SnakeAStar.Program.BlockSize) | 0), ((y * SnakeAStar.Program.BlockSize) | 0), SnakeAStar.Program.BlockSize, SnakeAStar.Program.BlockSize);
                    } else {
                        if (!Bridge.referenceEquals(SnakeAStar.ScreenManager.currentFillStyle, color)) {
                            context.fillStyle = color;
                            SnakeAStar.ScreenManager.currentFillStyle = color;
                        }
                        context.fillRect(((x * SnakeAStar.Program.BlockSize) | 0), ((y * SnakeAStar.Program.BlockSize) | 0), SnakeAStar.Program.BlockSize, SnakeAStar.Program.BlockSize);
                    }

                }
            }
        }
    });

    Bridge.define("SnakeAStar.Snake", {
        points: null,
        $ctor1: function (x, y, facing) {
            this.$initialize();
            var facingPoint = SnakeAStar.Point.getPoint(x, y, facing);
            this.points = function (_o2) {
                    _o2.add(facingPoint);
                    return _o2;
                }(new (System.Collections.Generic.List$1(SnakeAStar.Point))(2));
        },
        ctor: function (original) {
            this.$initialize();
            this.points = new (System.Collections.Generic.List$1(SnakeAStar.Point))(((original.points.getCount() + 1) | 0));
            this.points.addRange(original.points);
        },
        getHead: function () {
            return this.points.getItem(0);
        },
        setHead: function (value) {
            this.points.setItem(0, value);
        },
        getTails: function () {
            var points = new (System.Collections.Generic.List$1(System.Int32))();
            for (var index = (this.points.getCount() - 3) | 0; index >= 0 && index < this.points.getCount(); index = (index + 1) | 0) {
                var point = this.points.getItem(index);
                points.add(point.hashCodeNoFacing);
            }
            return points.toArray();
        },
        containsPoint: function (x, y) {
            var pointHashCodeNoFacing = (((x * 100000) | 0) + y) | 0;
            return this.containsPointWithOffset(pointHashCodeNoFacing, 0);
        },
        containsPointWithOffset: function (hashCodeNoFacing, offset) {
            var pointCount = (this.points.getCount() - offset) | 0;
            for (var i = 0; i < pointCount; i = (i + 1) | 0) {
                if (this.points.getItem(i).hashCodeNoFacing === hashCodeNoFacing) {
                    return true;
                }
            }
            return false;
        },
        insertPoint: function (movePoint) {
            this.points.insert(0, movePoint);
        },
        insertAndMove: function (movePoint) {
            this.points.insert(0, movePoint);
            var last = (this.points.getCount() - 1) | 0;
            this.points.removeAt(last);
        },
        setFacing: function (facing) {
            this.setHead(SnakeAStar.Point.getPoint(this.getHead().x, this.getHead().y, facing));

        }
    });
});
