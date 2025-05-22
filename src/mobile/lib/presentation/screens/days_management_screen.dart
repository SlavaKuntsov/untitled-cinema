import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../config/theme.dart';
import '../../domain/entities/day/day.dart';
import '../providers/day_management_provider.dart';

class DaysManagementScreen extends StatefulWidget {
  const DaysManagementScreen({super.key});

  @override
  State<DaysManagementScreen> createState() => _DaysManagementScreenState();
}

class _DaysManagementScreenState extends State<DaysManagementScreen> {
  bool _isLoading = true;
  String? _errorMessage;
  final _formKey = GlobalKey<FormState>();

  // Controllers for the create day form
  final TextEditingController _startDateController = TextEditingController();
  final TextEditingController _startTimeController = TextEditingController();
  final TextEditingController _endDateController = TextEditingController();
  final TextEditingController _endTimeController = TextEditingController();

  DateTime _startDateTime = DateTime.now();
  DateTime _endDateTime = DateTime.now().add(const Duration(hours: 24));

  @override
  void initState() {
    super.initState();
    _loadDays();
    _updateControllers();
  }

  void _updateControllers() {
    _startDateController.text = DateFormat('yyyy-MM-dd').format(_startDateTime);
    _startTimeController.text = DateFormat('HH:mm').format(_startDateTime);
    _endDateController.text = DateFormat('yyyy-MM-dd').format(_endDateTime);
    _endTimeController.text = DateFormat('HH:mm').format(_endDateTime);
  }

  @override
  void dispose() {
    _startDateController.dispose();
    _startTimeController.dispose();
    _endDateController.dispose();
    _endTimeController.dispose();
    super.dispose();
  }

  Future<void> _loadDays() async {
    final dayManagementProvider = Provider.of<DayManagementProvider>(
      context,
      listen: false,
    );

    setState(() {
      _isLoading = true;
      _errorMessage = null;
    });

    try {
      await dayManagementProvider.fetchAllDays();
      setState(() {
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Ошибка загрузки дней: ${e.toString()}';
        _isLoading = false;
      });
    }
  }

  Future<void> _deleteDay(Day day) async {
    final dayManagementProvider = Provider.of<DayManagementProvider>(
      context,
      listen: false,
    );

    try {
      await dayManagementProvider.deleteDay(day.id);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'День ${DateFormat('dd.MM.yyyy').format(day.startTime)} успешно удален',
          ),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при удалении дня: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _createDay() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    final dayManagementProvider = Provider.of<DayManagementProvider>(
      context,
      listen: false,
    );

    try {
      // Combine date and time for start
      final startDate = DateFormat(
        'yyyy-MM-dd',
      ).parse(_startDateController.text);
      final startTime = DateFormat('HH:mm').parse(_startTimeController.text);
      final startDateTime = DateTime(
        startDate.year,
        startDate.month,
        startDate.day,
        startTime.hour,
        startTime.minute,
      );

      // Combine date and time for end
      final endDate = DateFormat('yyyy-MM-dd').parse(_endDateController.text);
      final endTime = DateFormat('HH:mm').parse(_endTimeController.text);
      final endDateTime = DateTime(
        endDate.year,
        endDate.month,
        endDate.day,
        endTime.hour,
        endTime.minute,
      );

      final newDay = await dayManagementProvider.createDay(
        startDateTime,
        endDateTime,
      );

      // if (newDay != null) {
      // Reset form
      _startDateTime = DateTime.now();
      _endDateTime = DateTime.now().add(const Duration(hours: 24));
      _updateControllers();

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('День успешно добавлен'),
          backgroundColor: Colors.green,
        ),
      );

      Navigator.pop(context); // Close the dialog
      // } else {
      //   ScaffoldMessenger.of(context).showSnackBar(
      //     const SnackBar(
      //       content: Text('Ошибка при создании дня'),
      //       backgroundColor: Colors.red,
      //     ),
      //   );
      // }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Ошибка при создании дня: ${e.toString()}'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  Future<void> _showDeleteConfirmation(Day day) async {
    final dateFormat = DateFormat('dd.MM.yyyy');
    final timeFormat = DateFormat('HH:mm');

    return showDialog<void>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Подтверждение удаления'),
          content: SingleChildScrollView(
            child: Text(
              'Вы действительно хотите удалить день ${dateFormat.format(day.startTime)}?',
            ),
          ),
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
                _deleteDay(day);
              },
            ),
          ],
        );
      },
    );
  }

  Future<void> _selectStartDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: _startDateTime,
      firstDate: DateTime(2020),
      lastDate: DateTime(2030),
    );

    if (picked != null) {
      setState(() {
        _startDateTime = DateTime(
          picked.year,
          picked.month,
          picked.day,
          _startDateTime.hour,
          _startDateTime.minute,
        );
        _startDateController.text = DateFormat(
          'yyyy-MM-dd',
        ).format(_startDateTime);
      });
    }
  }

  Future<void> _selectStartTime(BuildContext context) async {
    final TimeOfDay? picked = await showTimePicker(
      context: context,
      initialTime: TimeOfDay(
        hour: _startDateTime.hour,
        minute: _startDateTime.minute,
      ),
    );

    if (picked != null) {
      setState(() {
        _startDateTime = DateTime(
          _startDateTime.year,
          _startDateTime.month,
          _startDateTime.day,
          picked.hour,
          picked.minute,
        );
        _startTimeController.text = DateFormat('HH:mm').format(_startDateTime);
      });
    }
  }

  Future<void> _selectEndDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: _endDateTime,
      firstDate: DateTime(2020),
      lastDate: DateTime(2030),
    );

    if (picked != null) {
      setState(() {
        _endDateTime = DateTime(
          picked.year,
          picked.month,
          picked.day,
          _endDateTime.hour,
          _endDateTime.minute,
        );
        _endDateController.text = DateFormat('yyyy-MM-dd').format(_endDateTime);
      });
    }
  }

  Future<void> _selectEndTime(BuildContext context) async {
    final TimeOfDay? picked = await showTimePicker(
      context: context,
      initialTime: TimeOfDay(
        hour: _endDateTime.hour,
        minute: _endDateTime.minute,
      ),
    );

    if (picked != null) {
      setState(() {
        _endDateTime = DateTime(
          _endDateTime.year,
          _endDateTime.month,
          _endDateTime.day,
          picked.hour,
          picked.minute,
        );
        _endTimeController.text = DateFormat('HH:mm').format(_endDateTime);
      });
    }
  }

  Future<void> _showCreateDayDialog() async {
    // Initialize controllers with current values
    _startDateTime = DateTime.now();
    _endDateTime = DateTime.now().add(const Duration(hours: 24));
    _updateControllers();

    return showDialog<void>(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Добавить новый день'),
          content: SingleChildScrollView(
            child: Form(
              key: _formKey,
              child: Column(
                // mainAxisSize: MainAxisSize.min,
                children: [
                  const Text(
                    'Начало дня:',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 22),
                  Row(
                    children: [
                      // Start Date
                      Expanded(
                        child: TextFormField(
                          controller: _startDateController,
                          decoration: InputDecoration(
                            labelText: 'Дата',
                            suffixIcon: IconButton(
                              icon: const Icon(Icons.calendar_today),
                              onPressed: () => _selectStartDate(context),
                            ),
                          ),
                          readOnly: true,
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Выберите дату';
                            }
                            return null;
                          },
                        ),
                      ),
                      const SizedBox(width: 16),
                      // Start Time
                      Expanded(
                        child: TextFormField(
                          controller: _startTimeController,
                          decoration: InputDecoration(
                            labelText: 'Время',
                            suffixIcon: IconButton(
                              icon: const Icon(Icons.access_time),
                              onPressed: () => _selectStartTime(context),
                            ),
                          ),
                          readOnly: true,
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Выберите время';
                            }
                            return null;
                          },
                        ),
                      ),
                    ],
                  ),

                  const SizedBox(height: 24),
                  const Text(
                    'Конец дня:',
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                  const SizedBox(height: 22),
                  Row(
                    children: [
                      // End Date
                      Expanded(
                        child: TextFormField(
                          controller: _endDateController,
                          decoration: InputDecoration(
                            labelText: 'Дата',
                            suffixIcon: IconButton(
                              icon: const Icon(Icons.calendar_today),
                              onPressed: () => _selectEndDate(context),
                            ),
                          ),
                          readOnly: true,
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Выберите дату';
                            }
                            return null;
                          },
                        ),
                      ),
                      const SizedBox(width: 16),
                      // End Time
                      Expanded(
                        child: TextFormField(
                          controller: _endTimeController,
                          decoration: InputDecoration(
                            labelText: 'Время',
                            suffixIcon: IconButton(
                              icon: const Icon(Icons.access_time),
                              onPressed: () => _selectEndTime(context),
                            ),
                          ),
                          readOnly: true,
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Выберите время';
                            }
                            return null;
                          },
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
          actions: <Widget>[
            TextButton(
              child: const Text('Отмена'),
              onPressed: () {
                Navigator.of(context).pop();
              },
            ),
            TextButton(child: const Text('Добавить'), onPressed: _createDay),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Управление днями'),
        backgroundColor: AppTheme.primaryColor,
      ),
      body: _buildBody(),
      floatingActionButton: FloatingActionButton(
        onPressed: _showCreateDayDialog,
        backgroundColor: AppTheme.accentColor,
        child: const Icon(Icons.add),
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
              onPressed: _loadDays,
              child: const Text('Повторить'),
            ),
          ],
        ),
      );
    }

    final dayManagementProvider = Provider.of<DayManagementProvider>(context);
    final days = dayManagementProvider.days;

    if (days.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Text('Нет доступных дней'),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _showCreateDayDialog,
              child: const Text('Добавить день'),
            ),
          ],
        ),
      );
    }

    return RefreshIndicator(
      onRefresh: _loadDays,
      child: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: days.length,
        itemBuilder: (context, index) {
          final day = days[index];
          return _buildDayCard(day);
        },
      ),
    );
  }

  Widget _buildDayCard(Day day) {
    final dateFormat = DateFormat('dd.MM.yyyy');
    final timeFormat = DateFormat('HH:mm');

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
                        'День: ${dateFormat.format(day.startTime)}',
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
                            Icons.access_time_filled,
                            size: 16,
                            color: Colors.blue,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            'Начало: ${timeFormat.format(day.startTime)}',
                            style: const TextStyle(fontSize: 14),
                          ),
                        ],
                      ),
                      const SizedBox(height: 4),
                      Row(
                        children: [
                          const Icon(
                            Icons.access_time,
                            size: 16,
                            color: Colors.orange,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            'Конец: ${timeFormat.format(day.endTime)}',
                            style: const TextStyle(fontSize: 14),
                          ),
                        ],
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Row(
              mainAxisAlignment: MainAxisAlignment.end,
              children: [
                TextButton.icon(
                  onPressed: () => _showDeleteConfirmation(day),
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
}
