import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../domain/entities/hall/hall.dart';
import '../../domain/entities/session/seat_type.dart';
import '../providers/hall_managment_provider.dart';
import '../providers/session_provider.dart';

class HallsManagementScreen extends StatefulWidget {
  const HallsManagementScreen({super.key});

  @override
  State<HallsManagementScreen> createState() => _HallsManagementScreenState();
}

class _HallsManagementScreenState extends State<HallsManagementScreen> {
  bool _isLoading = true;
  String? _errorMessage;
  List<SeatType>? _seatTypes;

  @override
  void initState() {
    super.initState();
    _loadHalls();
  }

  Future<void> _loadHalls() async {
    final hallManagementProvider = Provider.of<HallManagementProvider>(
      context,
      listen: false,
    );

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      await hallManagementProvider.fetchAllHalls();
      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Ошибка загрузки залов: ${e.toString()}';
        _isLoading = false;
      });
    }
  }

  Future<void> _loadSeatTypes(String hallId) async {
    final sessionProvider = Provider.of<SessionProvider>(
      context,
      listen: false,
    );

    try {
      final seatTypes = await sessionProvider.fetchSeatTypesByHallId(
        hallId: hallId,
      );
      setState(() {
        _seatTypes = seatTypes;
      });
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка загрузки типов мест: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _deleteHall(Hall hall) async {
    final hallManagementProvider = Provider.of<HallManagementProvider>(
      context,
      listen: false,
    );

    try {
      await hallManagementProvider.deleteHall(hall.id);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Зал "${hall.name}" успешно удален'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при удалении зала: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _showDeleteConfirmation(Hall hall) async {
    return showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Подтверждение удаления'),
          content: Text('Вы действительно хотите удалить зал "${hall.name}"?'),
          actions: <Widget>[
            TextButton(
              child: const Text('Отмена'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(
              child: const Text('Удалить', style: TextStyle(color: Colors.red)),
              onPressed: () {
                Navigator.of(context).pop();
                _deleteHall(hall);
              },
            ),
          ],
        );
      },
    );
  }

  Future<void> _showCreateHallDialog() async {
    return showDialog<void>(
      context: context,
      builder: (BuildContext context) {
        return const CreateHallDialog();
      },
    );
  }

  Future<void> _showEditHallDialog(Hall hall) async {
    return showDialog<void>(
      context: context,
      builder: (BuildContext context) {
        return EditHallDialog(hall: hall);
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Управление залами'),
        backgroundColor: AppTheme.primaryColor,
      ),
      body: _buildBody(),
      floatingActionButton: FloatingActionButton(
        onPressed: _showCreateHallDialog,
        backgroundColor: AppTheme.accentColor,
        child: const Icon(Icons.add, color: Colors.white),
      ),
    );
  }

  Widget _buildBody() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_errorMessage != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              _errorMessage!,
              style: const TextStyle(color: Colors.red),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadHalls,
              child: const Text('Повторить'),
            ),
          ],
        ),
      );
    }

    final hallManagementProvider = Provider.of<HallManagementProvider>(context);
    final halls = hallManagementProvider.halls;

    if (halls.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Нет доступных залов'),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _showCreateHallDialog,
              child: const Text('Добавить зал'),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadHalls,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: halls.length,
        itemBuilder: (context, index) {
          final hall = halls[index];
          return _buildHallCard(hall);
        },
      ),
    );
  }

  Widget _buildHallCard(Hall hall) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        hall.name,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                        overflow: TextOverflow.ellipsis,
                      ),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          const Icon(
                            Icons.event_seat,
                            size: 16,
                            color: Colors.blue,
                          ),
                          const SizedBox(width: 4),
                          Text('Всего мест: ${hall.totalSeats}'),
                        ],
                      ),
                    ],
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.refresh),
                  onPressed: () => _loadSeatTypes(hall.id),
                  tooltip: 'Обновить типы мест',
                ),
              ],
            ),
            if (_seatTypes != null && _seatTypes!.isNotEmpty) ...[
              const SizedBox(height: 8),
              Row(
                children: [
                  const Text(
                    'Типы мест: ',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  Expanded(
                    child: Wrap(
                      spacing: 8,
                      children:
                          _seatTypes!.map((type) {
                            return Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: _getSeatColor(type.id),
                                borderRadius: BorderRadius.circular(4),
                                border: Border.all(color: Colors.grey[400]!),
                              ),
                              child: Text(
                                type.name,
                                style: TextStyle(
                                  color:
                                      type.id == "2"
                                          ? Colors.white
                                          : Colors.black87,
                                  fontWeight: FontWeight.bold,
                                ),
                              ),
                            );
                          }).toList(),
                    ),
                  ),
                ],
              ),
            ],
            const SizedBox(height: 16),
            // Схема зала
            Container(
              height: 200,
              width: double.infinity,
              decoration: BoxDecoration(
                color: Colors.grey[50],
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.grey[300]!),
              ),
              child: Padding(
                padding: const EdgeInsets.all(8.0),
                child: _buildSeatMap(hall.seatsArray),
              ),
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton.icon(
                  onPressed: () => _showEditHallDialog(hall),
                  icon: const Icon(Icons.edit, color: Colors.blue),
                  label: const Text(
                    'Редактировать',
                    style: TextStyle(color: Colors.blue),
                  ),
                ),
                const SizedBox(width: 8),
                TextButton.icon(
                  onPressed: () => _showDeleteConfirmation(hall),
                  icon: const Icon(Icons.delete, color: Colors.red),
                  label: const Text(
                    'Удалить',
                    style: TextStyle(color: Colors.red),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSeatMap(List<List<int>> seatsArray) {
    if (seatsArray.isEmpty)
      return const Center(child: Text('Нет данных о местах'));

    return LayoutBuilder(
      builder: (context, constraints) {
        final rows = seatsArray.length;
        final maxCols = seatsArray
            .map((row) => row.length)
            .reduce((a, b) => a > b ? a : b);

        final cellWidth = constraints.maxWidth / maxCols;
        final cellHeight = constraints.maxHeight / rows;
        final cellSize = cellWidth < cellHeight ? cellWidth : cellHeight;

        return Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children:
                seatsArray.asMap().entries.map((rowEntry) {
                  final rowIndex = rowEntry.key;
                  final row = rowEntry.value;

                  return Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children:
                        row.asMap().entries.map((colEntry) {
                          final colIndex = colEntry.key;
                          final seatType = colEntry.value;

                          return Container(
                            width: cellSize * 0.8,
                            height: cellSize * 0.8,
                            margin: EdgeInsets.all(cellSize * 0.05),
                            decoration: BoxDecoration(
                              color: _getSeatColor(seatType.toString()),
                              borderRadius: BorderRadius.circular(4),
                              border:
                                  seatType != -1
                                      ? Border.all(color: Colors.grey[400]!)
                                      : null,
                            ),
                            child:
                                seatType != -1
                                    ? Center(
                                      child: Text(
                                        '${rowIndex + 1}-${colIndex + 1}',
                                        style: TextStyle(
                                          fontSize: cellSize * 0.2,
                                          fontWeight: FontWeight.bold,
                                          color:
                                              seatType == "2"
                                                  ? Colors.white
                                                  : Colors.black87,
                                        ),
                                        textAlign: TextAlign.center,
                                      ),
                                    )
                                    : null,
                          );
                        }).toList(),
                  );
                }).toList(),
          ),
        );
      },
    );
  }

  Color _getSeatColor(String seatType) {
    switch (seatType) {
      case '-1':
        return Colors.transparent; // Проход
      case '1':
        return Colors.blue[100]!; // Обычное место
      case '2':
        return Colors.amber[600]!; // VIP место
      default:
        return Colors.grey[300]!;
    }
  }
}

// Диалог создания зала
class CreateHallDialog extends StatefulWidget {
  const CreateHallDialog({Key? key}) : super(key: key);

  @override
  State<CreateHallDialog> createState() => _CreateHallDialogState();
}

class _CreateHallDialogState extends State<CreateHallDialog> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _rowsController = TextEditingController();
  final _seatsPerRowController = TextEditingController();
  final _customSeatsController = TextEditingController(
    text: '[[-1,1,1,-1],[1,1,1,1]]',
  );

  int _selectedType = 0; // 0 - простой, 1 - кастомный

  @override
  void dispose() {
    _nameController.dispose();
    _rowsController.dispose();
    _seatsPerRowController.dispose();
    _customSeatsController.dispose();
    super.dispose();
  }

  Future<void> _createHall() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final hallManagementProvider = Provider.of<HallManagementProvider>(
      context,
      listen: false,
    );

    try {
      Hall? newHall;

      if (_selectedType == 0) {
        // Простой зал
        newHall = await hallManagementProvider.createSimpleHall(
          _nameController.text,
          int.parse(_rowsController.text),
          int.parse(_seatsPerRowController.text),
        );
      } else {
        // Кастомный зал
        newHall = await hallManagementProvider.createCustomHall(
          _nameController.text,
          _customSeatsController.text,
        );
      }

      if (newHall != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Зал успешно создан'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при создании зала: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Создать новый зал'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _nameController,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'Название зала',
                  labelStyle: TextStyle(color: Colors.white),
                  enabledBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Введите название зала';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              SegmentedButton<int>(
                segments: const [
                  ButtonSegment(value: 0, label: Text('Простой')),
                  ButtonSegment(value: 1, label: Text('Кастомный')),
                ],
                selected: {_selectedType},
                onSelectionChanged: (Set<int> newSelection) {
                  setState(() {
                    _selectedType = newSelection.first;
                  });
                },
              ),
              const SizedBox(height: 16),
              if (_selectedType == 0) ...[
                TextFormField(
                  controller: _rowsController,
                  style: const TextStyle(color: Colors.white),
                  decoration: const InputDecoration(
                    labelText: 'Количество рядов',
                    labelStyle: TextStyle(color: Colors.white),
                    enabledBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                  ),
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите количество рядов';
                    }
                    if (int.tryParse(value) == null || int.parse(value) <= 0) {
                      return 'Введите корректное число';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _seatsPerRowController,
                  style: const TextStyle(color: Colors.white),
                  decoration: const InputDecoration(
                    labelText: 'Мест в ряду',
                    labelStyle: TextStyle(color: Colors.white),
                    enabledBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                  ),
                  keyboardType: TextInputType.number,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите количество мест в ряду';
                    }
                    if (int.tryParse(value) == null || int.parse(value) <= 0) {
                      return 'Введите корректное число';
                    }
                    return null;
                  },
                ),
              ] else ...[
                TextFormField(
                  cursorColor: Colors.white,
                  controller: _customSeatsController,
                  style: const TextStyle(color: Colors.white),
                  decoration: const InputDecoration(
                    labelText: 'JSON схема мест',
                    helperText: '-1: проход, 1: обычное место, 2: VIP место',
                    labelStyle: TextStyle(color: Colors.white),
                    hintStyle: TextStyle(color: Colors.white70),
                    helperStyle: TextStyle(color: Colors.white70),
                    enabledBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                    focusedBorder: OutlineInputBorder(
                      borderSide: BorderSide(color: Colors.white),
                    ),
                  ),
                  maxLines: 4,
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Введите схему мест';
                    }
                    try {
                      json.decode(value);
                    } catch (e) {
                      return 'Некорректный JSON формат';
                    }
                    return null;
                  },
                ),
              ],
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Отмена'),
        ),
        TextButton(onPressed: _createHall, child: const Text('Создать')),
      ],
    );
  }
}

// Диалог редактирования зала
class EditHallDialog extends StatefulWidget {
  final Hall hall;

  const EditHallDialog({Key? key, required this.hall}) : super(key: key);

  @override
  State<EditHallDialog> createState() => _EditHallDialogState();
}

class _EditHallDialogState extends State<EditHallDialog> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _seatsController = TextEditingController();
  List<SeatType>? _seatTypes;

  @override
  void initState() {
    super.initState();
    _nameController.text = widget.hall.name;
    _seatsController.text = json.encode(widget.hall.seatsArray);
    _loadSeatTypes();
  }

  Future<void> _loadSeatTypes() async {
    final sessionProvider = Provider.of<SessionProvider>(
      context,
      listen: false,
    );

    try {
      final seatTypes = await sessionProvider.fetchSeatTypesByHallId(
        hallId: widget.hall.id,
      );
      setState(() {
        _seatTypes = seatTypes;
      });
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка загрузки типов мест: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _seatsController.dispose();
    super.dispose();
  }

  Color _getSeatColor(String seatType) {
    switch (seatType) {
      case '-1':
        return Colors.transparent; // Проход
      case '1':
        return Colors.blue[100]!; // Обычное место
      case '2':
        return Colors.amber[600]!; // VIP место
      default:
        return Colors.grey[300]!;
    }
  }

  Future<void> _updateHall() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final hallManagementProvider = Provider.of<HallManagementProvider>(
      context,
      listen: false,
    );

    try {
      final updatedHall = await hallManagementProvider.updateHall(
        widget.hall.id,
        _nameController.text,
        _seatsController.text,
      );

      if (updatedHall != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Зал успешно обновлен'),
            backgroundColor: Colors.green,
          ),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при обновлении зала: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: const Text('Редактировать зал'),
      content: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextFormField(
                controller: _nameController,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'Название зала',
                  labelStyle: TextStyle(color: Colors.white),
                  enabledBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Введите название зала';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              if (_seatTypes != null && _seatTypes!.isNotEmpty) ...[
                const Text('Типы мест:', style: TextStyle(color: Colors.white)),
                const SizedBox(height: 8),
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    border: Border.all(color: Colors.white),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children:
                        _seatTypes!.asMap().entries.map((entry) {
                          final index = entry.key;
                          final type = entry.value;
                          return Padding(
                            padding: const EdgeInsets.symmetric(vertical: 4),
                            child: Row(
                              children: [
                                // Container(
                                //   width: 24,
                                //   height: 24,
                                //   decoration: BoxDecoration(
                                //     color: _getSeatColor(type.id),
                                //     borderRadius: BorderRadius.circular(4),
                                //     border: Border.all(color: Colors.grey[400]!),
                                //   ),
                                // ),
                                // const SizedBox(width: 8),
                                Text(
                                  '${index + 1}. ${type.name}',
                                  style: TextStyle(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ],
                            ),
                          );
                        }).toList(),
                  ),
                ),
                const SizedBox(height: 16),
              ],
              TextFormField(
                controller: _seatsController,
                style: const TextStyle(color: Colors.white),
                decoration: const InputDecoration(
                  labelText: 'JSON схема мест',
                  labelStyle: TextStyle(color: Colors.white),
                  enabledBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                  focusedBorder: OutlineInputBorder(
                    borderSide: BorderSide(color: Colors.white),
                  ),
                ),
                maxLines: 6,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Введите схему мест';
                  }
                  try {
                    json.decode(value);
                  } catch (e) {
                    return 'Некорректный JSON формат';
                  }
                  return null;
                },
              ),
            ],
          ),
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Отмена'),
        ),
        TextButton(onPressed: _updateHall, child: const Text('Сохранить')),
      ],
    );
  }
}
