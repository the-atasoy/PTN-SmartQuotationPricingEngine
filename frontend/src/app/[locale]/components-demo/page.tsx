"use client";

import React, { useState } from 'react';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Badge } from '@/components/ui/Badge';
import { Spinner } from '@/components/ui/Spinner';
import { Modal } from '@/components/ui/Modal';

export default function ComponentsDemoPage() {
  const [isModalOpen, setIsModalOpen] = useState(false);

  return (
    <div className="container mx-auto p-8 max-w-4xl space-y-12">
      <h1 className="text-3xl font-bold mb-8">UI Components Demo</h1>
      
      <section className="space-y-4">
        <h2 className="text-xl font-semibold border-b pb-2">Buttons</h2>
        <div className="flex flex-wrap gap-4 items-center">
          <Button variant="primary">Primary Button</Button>
          <Button variant="secondary">Secondary Button</Button>
          <Button variant="danger">Danger Button</Button>
          <Button variant="primary" isLoading>Loading State</Button>
          <Button variant="primary" disabled>Disabled State</Button>
        </div>
      </section>

      <section className="space-y-4">
        <h2 className="text-xl font-semibold border-b pb-2">Inputs</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 max-w-2xl">
          <Input label="Standard Input" placeholder="Type here..." helperText="This is a normal input field." />
          <Input label="Error Input" placeholder="Invalid input" error="This field is required." />
          <Input label="Disabled Input" placeholder="Disabled" disabled />
        </div>
      </section>

      <section className="space-y-4">
        <h2 className="text-xl font-semibold border-b pb-2">Badges</h2>
        <div className="flex flex-wrap gap-4">
          <Badge variant="default">Default</Badge>
          <Badge variant="success">Success</Badge>
          <Badge variant="warning">Warning</Badge>
          <Badge variant="danger">Danger</Badge>
        </div>
      </section>

      <section className="space-y-4">
        <h2 className="text-xl font-semibold border-b pb-2">Spinners</h2>
        <div className="flex flex-wrap gap-6 items-center">
          <Spinner size="sm" className="text-blue-600" />
          <Spinner size="md" className="text-green-600" />
          <Spinner size="lg" className="text-yellow-600" />
          <Spinner size="xl" className="text-red-600" />
        </div>
      </section>

      <section className="space-y-4">
        <h2 className="text-xl font-semibold border-b pb-2">Modal</h2>
        <Button onClick={() => setIsModalOpen(true)}>Open Modal</Button>
        <Modal 
          isOpen={isModalOpen} 
          onClose={() => setIsModalOpen(false)}
          title="Example Modal"
        >
          <div className="space-y-4">
            <p className="text-gray-600">
              This is a demonstration of the modal component. It traps focus, prevents background scrolling, and can be closed with the Escape key or by clicking the overlay.
            </p>
            <div className="flex justify-end gap-2 pt-4">
              <Button variant="secondary" onClick={() => setIsModalOpen(false)}>Cancel</Button>
              <Button variant="primary" onClick={() => setIsModalOpen(false)}>Confirm</Button>
            </div>
          </div>
        </Modal>
      </section>

    </div>
  );
}
